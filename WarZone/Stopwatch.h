#pragma once

#include "Platform.h"

#ifdef _WIN32
class StopWatch { 
  LARGE_INTEGER frequency_;
  LARGE_INTEGER startTime_; 
  LARGE_INTEGER stopTime_; 

public: 
StopWatch() {
  if (!::QueryPerformanceFrequency(&frequency_)) throw "Error with QueryPerformanceFrequency"; 
} 

void Start() {
   ::QueryPerformanceCounter(&startTime_); 
} 

void Stop() {
   ::QueryPerformanceCounter(&stopTime_); 
} 

float MilliSeconds() const { 
   float v = ((float)stopTime_.QuadPart - (float)startTime_.QuadPart) / ((float)frequency_.QuadPart / 1000.0f);
   return v;
} 

int64_t Ticks() const { 
  return (int64_t)(stopTime_.QuadPart - startTime_.QuadPart);
} 
}; 
#else
#include <chrono>
class StopWatch { 
  std::chrono::high_resolution_clock::time_point startTime_; 
  std::chrono::high_resolution_clock::time_point stopTime_; 

public: 
StopWatch() {} 

void Start() {
   startTime_ = std::chrono::high_resolution_clock::now(); 
} 

void Stop() {
   stopTime_ = std::chrono::high_resolution_clock::now(); 
} 

float MilliSeconds() const { 
   return std::chrono::duration<float, std::milli>(stopTime_ - startTime_).count();
} 

int64_t Ticks() const { 
  return std::chrono::duration_cast<std::chrono::microseconds>(stopTime_ - startTime_).count();
} 
}; 
#endif
