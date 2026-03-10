using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FrameWork
{
    public static class CountryBlockPolicy
    {
        private const string EnableEnvironmentVariable = "PROJECTWAR_COUNTRY_BLOCK_ENABLED";
        private const string DataRootEnvironmentVariable = "PROJECTWAR_GEOIP_CSV_ROOT";
        private const string DataRootBase64EnvironmentVariable = "PROJECTWAR_GEOIP_CSV_ROOT_B64";
        private const string BlockedCountriesEnvironmentVariable = "PROJECTWAR_BLOCKED_COUNTRIES";
        private const string BlockedCountriesBase64EnvironmentVariable = "PROJECTWAR_BLOCKED_COUNTRIES_B64";
        private const string FailClosedEnvironmentVariable = "PROJECTWAR_COUNTRY_BLOCK_FAIL_CLOSED";

        private static readonly object InitializationLock = new object();
        private static PolicyState _state;
        private static bool _initialized;

        public static void Warmup()
        {
            EnsureInitialized();
        }

        public static bool ShouldReject(EndPoint remoteEndPoint, out string countryCode, out string reason)
        {
            countryCode = string.Empty;
            reason = string.Empty;

            IPEndPoint ipEndPoint = remoteEndPoint as IPEndPoint;
            if (ipEndPoint == null)
                return false;

            return ShouldReject(ipEndPoint.Address, out countryCode, out reason);
        }

        public static bool ShouldReject(IPAddress address, out string countryCode, out string reason)
        {
            countryCode = string.Empty;
            reason = string.Empty;

            PolicyState state = EnsureInitialized();
            if (address == null || state == null || !state.Enabled)
                return false;

            if (!state.Loaded)
            {
                if (!state.FailClosed)
                    return false;

                reason = "country block enabled but GeoIP data could not be loaded";
                return true;
            }

            IPAddress normalizedAddress = NormalizeAddress(address);
            string matchedCountryCode;
            if (!TryResolveCountryCode(state, normalizedAddress, out matchedCountryCode))
                return false;

            if (!state.BlockedCountries.Contains(matchedCountryCode))
                return false;

            countryCode = matchedCountryCode;
            reason = "blocked country " + matchedCountryCode;
            return true;
        }

        private static PolicyState EnsureInitialized()
        {
            if (_initialized)
                return _state;

            lock (InitializationLock)
            {
                if (_initialized)
                    return _state;

                _state = LoadState();
                _initialized = true;
                return _state;
            }
        }

        private static PolicyState LoadState()
        {
            bool enabled = IsTruthy(Environment.GetEnvironmentVariable(EnableEnvironmentVariable));
            bool failClosed = IsTruthy(Environment.GetEnvironmentVariable(FailClosedEnvironmentVariable));
            HashSet<string> blockedCountries = ReadBlockedCountries();
            string dataRoot = ReadEnvironmentValue(DataRootBase64EnvironmentVariable, DataRootEnvironmentVariable);

            if (!enabled)
                return new PolicyState(false, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());

            if (blockedCountries.Count == 0)
            {
                Log.Error("CountryBlockPolicy", "Country blocking is enabled but no blocked countries were configured.");
                return new PolicyState(true, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());
            }

            if (string.IsNullOrWhiteSpace(dataRoot))
            {
                Log.Error("CountryBlockPolicy", "Country blocking is enabled but no GeoIP CSV root was provided.");
                return new PolicyState(true, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());
            }

            if (!Directory.Exists(dataRoot))
            {
                Log.Error("CountryBlockPolicy", "Country blocking is enabled but the GeoIP CSV root does not exist: " + dataRoot);
                return new PolicyState(true, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());
            }

            try
            {
                string locationsPath = FindGeoIpFile(dataRoot, "*Country-Locations-en.csv");
                string ipv4Path = FindGeoIpFile(dataRoot, "*Country-Blocks-IPv4.csv");
                string ipv6Path = FindGeoIpFile(dataRoot, "*Country-Blocks-IPv6.csv");

                if (string.IsNullOrWhiteSpace(locationsPath) || string.IsNullOrWhiteSpace(ipv4Path))
                {
                    Log.Error("CountryBlockPolicy", "Country blocking is enabled but the required GeoIP CSV files were not found under " + dataRoot + ".");
                    return new PolicyState(true, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());
                }

                Dictionary<int, string> countryCodesByGeonameId = LoadCountryCodes(locationsPath);
                List<IpRangeV4> ipv4Ranges = LoadIpv4Ranges(ipv4Path, blockedCountries, countryCodesByGeonameId);
                List<IpRangeV6> ipv6Ranges = string.IsNullOrWhiteSpace(ipv6Path)
                    ? new List<IpRangeV6>()
                    : LoadIpv6Ranges(ipv6Path, blockedCountries, countryCodesByGeonameId);

                Log.Info(
                    "CountryBlockPolicy",
                    "Loaded country block policy for "
                    + string.Join(",", blockedCountries.OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
                    + " using "
                    + ipv4Ranges.Count.ToString(CultureInfo.InvariantCulture)
                    + " IPv4 range(s) and "
                    + ipv6Ranges.Count.ToString(CultureInfo.InvariantCulture)
                    + " IPv6 range(s).");

                return new PolicyState(true, true, true, failClosed, blockedCountries, ipv4Ranges, ipv6Ranges);
            }
            catch (Exception ex)
            {
                Log.Error("CountryBlockPolicy", "Failed to initialize GeoIP country blocking: " + ex);
                return new PolicyState(true, false, false, failClosed, blockedCountries, new List<IpRangeV4>(), new List<IpRangeV6>());
            }
        }

        private static HashSet<string> ReadBlockedCountries()
        {
            string rawValue = ReadEnvironmentValue(BlockedCountriesBase64EnvironmentVariable, BlockedCountriesEnvironmentVariable);
            if (string.IsNullOrWhiteSpace(rawValue))
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            return new HashSet<string>(
                rawValue
                    .Split(new[] { ',', ';', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim().ToUpperInvariant())
                    .Where(value => value.Length == 2),
                StringComparer.OrdinalIgnoreCase);
        }

        private static string ReadEnvironmentValue(string base64EnvironmentVariable, string plainEnvironmentVariable)
        {
            string base64Value = Environment.GetEnvironmentVariable(base64EnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(base64Value))
            {
                try
                {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(base64Value.Trim()));
                }
                catch (FormatException ex)
                {
                    Log.Error("CountryBlockPolicy", "Invalid base64 value in " + base64EnvironmentVariable + ": " + ex.Message);
                }
            }

            return Environment.GetEnvironmentVariable(plainEnvironmentVariable);
        }

        private static string FindGeoIpFile(string rootPath, string pattern)
        {
            return Directory.GetFiles(rootPath, pattern, SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
        }

        private static Dictionary<int, string> LoadCountryCodes(string locationsPath)
        {
            Dictionary<int, string> countryCodesByGeonameId = new Dictionary<int, string>();
            CsvHeader header = null;

            foreach (string line in File.ReadLines(locationsPath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                List<string> columns = ParseCsvLine(line);
                if (header == null)
                {
                    header = new CsvHeader(columns);
                    continue;
                }

                int geonameId;
                if (!TryGetInt(columns, header, "geoname_id", out geonameId))
                    continue;

                string countryCode = GetColumn(columns, header, "country_iso_code");
                if (string.IsNullOrWhiteSpace(countryCode))
                    continue;

                countryCodesByGeonameId[geonameId] = countryCode.Trim().ToUpperInvariant();
            }

            return countryCodesByGeonameId;
        }

        private static List<IpRangeV4> LoadIpv4Ranges(string blocksPath, HashSet<string> blockedCountries, Dictionary<int, string> countryCodesByGeonameId)
        {
            List<IpRangeV4> ranges = new List<IpRangeV4>();
            CsvHeader header = null;

            foreach (string line in File.ReadLines(blocksPath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                List<string> columns = ParseCsvLine(line);
                if (header == null)
                {
                    header = new CsvHeader(columns);
                    continue;
                }

                string countryCode = ResolveCountryCode(columns, header, countryCodesByGeonameId);
                if (string.IsNullOrWhiteSpace(countryCode) || !blockedCountries.Contains(countryCode))
                    continue;

                uint start;
                uint end;
                if (!TryParseIpv4Range(GetColumn(columns, header, "network"), out start, out end))
                    continue;

                ranges.Add(new IpRangeV4(start, end, countryCode));
            }

            ranges.Sort((left, right) => left.Start.CompareTo(right.Start));
            return ranges;
        }

        private static List<IpRangeV6> LoadIpv6Ranges(string blocksPath, HashSet<string> blockedCountries, Dictionary<int, string> countryCodesByGeonameId)
        {
            List<IpRangeV6> ranges = new List<IpRangeV6>();
            CsvHeader header = null;

            foreach (string line in File.ReadLines(blocksPath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                List<string> columns = ParseCsvLine(line);
                if (header == null)
                {
                    header = new CsvHeader(columns);
                    continue;
                }

                string countryCode = ResolveCountryCode(columns, header, countryCodesByGeonameId);
                if (string.IsNullOrWhiteSpace(countryCode) || !blockedCountries.Contains(countryCode))
                    continue;

                byte[] start;
                byte[] end;
                if (!TryParseIpv6Range(GetColumn(columns, header, "network"), out start, out end))
                    continue;

                ranges.Add(new IpRangeV6(start, end, countryCode));
            }

            ranges.Sort((left, right) => CompareAddressBytes(left.Start, right.Start));
            return ranges;
        }

        private static string ResolveCountryCode(IList<string> columns, CsvHeader header, Dictionary<int, string> countryCodesByGeonameId)
        {
            int geonameId;
            string countryCode;

            if (TryGetInt(columns, header, "represented_country_geoname_id", out geonameId)
                && countryCodesByGeonameId.TryGetValue(geonameId, out countryCode)
                && !string.IsNullOrWhiteSpace(countryCode))
                return countryCode;

            if (TryGetInt(columns, header, "registered_country_geoname_id", out geonameId)
                && countryCodesByGeonameId.TryGetValue(geonameId, out countryCode)
                && !string.IsNullOrWhiteSpace(countryCode))
                return countryCode;

            if (TryGetInt(columns, header, "geoname_id", out geonameId)
                && countryCodesByGeonameId.TryGetValue(geonameId, out countryCode)
                && !string.IsNullOrWhiteSpace(countryCode))
                return countryCode;

            return string.Empty;
        }

        private static bool TryResolveCountryCode(PolicyState state, IPAddress address, out string countryCode)
        {
            countryCode = string.Empty;

            if (address == null)
                return false;

            if (address.AddressFamily == AddressFamily.InterNetwork)
                return TryResolveIpv4CountryCode(state.Ipv4Ranges, address, out countryCode);

            if (address.AddressFamily == AddressFamily.InterNetworkV6)
                return TryResolveIpv6CountryCode(state.Ipv6Ranges, address, out countryCode);

            return false;
        }

        private static bool TryResolveIpv4CountryCode(IList<IpRangeV4> ranges, IPAddress address, out string countryCode)
        {
            countryCode = string.Empty;
            if (ranges == null || ranges.Count == 0)
                return false;

            uint value = ToUInt32(address.GetAddressBytes());
            int low = 0;
            int high = ranges.Count - 1;

            while (low <= high)
            {
                int mid = low + ((high - low) / 2);
                IpRangeV4 range = ranges[mid];
                if (value < range.Start)
                {
                    high = mid - 1;
                    continue;
                }

                if (value > range.End)
                {
                    low = mid + 1;
                    continue;
                }

                countryCode = range.CountryCode;
                return true;
            }

            return false;
        }

        private static bool TryResolveIpv6CountryCode(IList<IpRangeV6> ranges, IPAddress address, out string countryCode)
        {
            countryCode = string.Empty;
            if (ranges == null || ranges.Count == 0)
                return false;

            byte[] value = address.GetAddressBytes();
            int low = 0;
            int high = ranges.Count - 1;

            while (low <= high)
            {
                int mid = low + ((high - low) / 2);
                IpRangeV6 range = ranges[mid];
                if (CompareAddressBytes(value, range.Start) < 0)
                {
                    high = mid - 1;
                    continue;
                }

                if (CompareAddressBytes(value, range.End) > 0)
                {
                    low = mid + 1;
                    continue;
                }

                countryCode = range.CountryCode;
                return true;
            }

            return false;
        }

        private static IPAddress NormalizeAddress(IPAddress address)
        {
            if (address == null)
                return null;

            if (address.AddressFamily == AddressFamily.InterNetworkV6 && address.IsIPv4MappedToIPv6)
                return address.MapToIPv4();

            return address;
        }

        private static bool TryParseIpv4Range(string networkText, out uint start, out uint end)
        {
            start = 0;
            end = 0;

            IPAddress address;
            int prefixLength;
            if (!TryParseCidr(networkText, AddressFamily.InterNetwork, out address, out prefixLength))
                return false;

            byte[] addressBytes = address.GetAddressBytes();
            byte[] startBytes = ApplyPrefix(addressBytes, prefixLength);
            byte[] endBytes = BuildRangeEnd(startBytes, prefixLength);
            start = ToUInt32(startBytes);
            end = ToUInt32(endBytes);
            return true;
        }

        private static bool TryParseIpv6Range(string networkText, out byte[] start, out byte[] end)
        {
            start = null;
            end = null;

            IPAddress address;
            int prefixLength;
            if (!TryParseCidr(networkText, AddressFamily.InterNetworkV6, out address, out prefixLength))
                return false;

            byte[] addressBytes = address.GetAddressBytes();
            start = ApplyPrefix(addressBytes, prefixLength);
            end = BuildRangeEnd(start, prefixLength);
            return true;
        }

        private static bool TryParseCidr(string networkText, AddressFamily expectedFamily, out IPAddress address, out int prefixLength)
        {
            address = null;
            prefixLength = 0;

            if (string.IsNullOrWhiteSpace(networkText))
                return false;

            string[] parts = networkText.Trim().Split('/');
            if (!IPAddress.TryParse(parts[0], out address) || address.AddressFamily != expectedFamily)
                return false;

            int defaultPrefix = expectedFamily == AddressFamily.InterNetwork ? 32 : 128;
            if (parts.Length == 1)
            {
                prefixLength = defaultPrefix;
                return true;
            }

            if (parts.Length != 2 || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out prefixLength))
                return false;

            if (prefixLength < 0 || prefixLength > defaultPrefix)
                return false;

            return true;
        }

        private static byte[] ApplyPrefix(byte[] addressBytes, int prefixLength)
        {
            byte[] maskedBytes = new byte[addressBytes.Length];
            Buffer.BlockCopy(addressBytes, 0, maskedBytes, 0, addressBytes.Length);

            int fullBytes = prefixLength / 8;
            int remainingBits = prefixLength % 8;

            if (remainingBits > 0 && fullBytes < maskedBytes.Length)
            {
                int maskValue = 0xFF << (8 - remainingBits);
                maskedBytes[fullBytes] = (byte)(maskedBytes[fullBytes] & maskValue);
                ++fullBytes;
            }

            for (int index = fullBytes; index < maskedBytes.Length; ++index)
                maskedBytes[index] = 0;

            return maskedBytes;
        }

        private static byte[] BuildRangeEnd(byte[] startBytes, int prefixLength)
        {
            byte[] endBytes = new byte[startBytes.Length];
            Buffer.BlockCopy(startBytes, 0, endBytes, 0, startBytes.Length);

            int fullBytes = prefixLength / 8;
            int remainingBits = prefixLength % 8;

            if (remainingBits > 0 && fullBytes < endBytes.Length)
            {
                int hostMask = (1 << (8 - remainingBits)) - 1;
                endBytes[fullBytes] = (byte)(endBytes[fullBytes] | hostMask);
                ++fullBytes;
            }

            for (int index = fullBytes; index < endBytes.Length; ++index)
                endBytes[index] = 0xFF;

            return endBytes;
        }

        private static uint ToUInt32(byte[] bytes)
        {
            return ((uint)bytes[0] << 24)
                | ((uint)bytes[1] << 16)
                | ((uint)bytes[2] << 8)
                | bytes[3];
        }

        private static int CompareAddressBytes(byte[] left, byte[] right)
        {
            int length = Math.Min(left == null ? 0 : left.Length, right == null ? 0 : right.Length);
            for (int index = 0; index < length; ++index)
            {
                if (left[index] == right[index])
                    continue;

                return left[index] < right[index] ? -1 : 1;
            }

            if (left == null || right == null)
                return left == right ? 0 : left == null ? -1 : 1;

            if (left.Length == right.Length)
                return 0;

            return left.Length < right.Length ? -1 : 1;
        }

        private static List<string> ParseCsvLine(string line)
        {
            List<string> columns = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            foreach (char character in line)
            {
                if (character == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (character == ',' && !inQuotes)
                {
                    columns.Add(current.ToString());
                    current.Length = 0;
                    continue;
                }

                current.Append(character);
            }

            columns.Add(current.ToString());
            return columns;
        }

        private static bool TryGetInt(IList<string> columns, CsvHeader header, string columnName, out int value)
        {
            value = 0;
            string rawValue = GetColumn(columns, header, columnName);
            return !string.IsNullOrWhiteSpace(rawValue)
                && int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        private static string GetColumn(IList<string> columns, CsvHeader header, string columnName)
        {
            int index = header.GetIndex(columnName);
            if (index < 0 || index >= columns.Count)
                return string.Empty;

            return columns[index];
        }

        private static bool IsTruthy(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Equals("1", StringComparison.OrdinalIgnoreCase)
                || value.Equals("true", StringComparison.OrdinalIgnoreCase)
                || value.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || value.Equals("on", StringComparison.OrdinalIgnoreCase);
        }

        private sealed class CsvHeader
        {
            private readonly Dictionary<string, int> _indices;

            public CsvHeader(IList<string> columns)
            {
                _indices = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int index = 0; index < columns.Count; ++index)
                {
                    string name = columns[index] == null ? string.Empty : columns[index].Trim();
                    if (!_indices.ContainsKey(name))
                        _indices[name] = index;
                }
            }

            public int GetIndex(string name)
            {
                int index;
                return _indices.TryGetValue(name, out index) ? index : -1;
            }
        }

        private sealed class PolicyState
        {
            public PolicyState(bool enabled, bool loaded, bool hasData, bool failClosed, HashSet<string> blockedCountries, List<IpRangeV4> ipv4Ranges, List<IpRangeV6> ipv6Ranges)
            {
                Enabled = enabled;
                Loaded = loaded && hasData;
                FailClosed = failClosed;
                BlockedCountries = blockedCountries ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                Ipv4Ranges = ipv4Ranges ?? new List<IpRangeV4>();
                Ipv6Ranges = ipv6Ranges ?? new List<IpRangeV6>();
            }

            public bool Enabled { get; private set; }
            public bool Loaded { get; private set; }
            public bool FailClosed { get; private set; }
            public HashSet<string> BlockedCountries { get; private set; }
            public List<IpRangeV4> Ipv4Ranges { get; private set; }
            public List<IpRangeV6> Ipv6Ranges { get; private set; }
        }

        private sealed class IpRangeV4
        {
            public IpRangeV4(uint start, uint end, string countryCode)
            {
                Start = start;
                End = end;
                CountryCode = countryCode ?? string.Empty;
            }

            public uint Start { get; private set; }
            public uint End { get; private set; }
            public string CountryCode { get; private set; }
        }

        private sealed class IpRangeV6
        {
            public IpRangeV6(byte[] start, byte[] end, string countryCode)
            {
                Start = start ?? new byte[16];
                End = end ?? new byte[16];
                CountryCode = countryCode ?? string.Empty;
            }

            public byte[] Start { get; private set; }
            public byte[] End { get; private set; }
            public string CountryCode { get; private set; }
        }
    }
}
