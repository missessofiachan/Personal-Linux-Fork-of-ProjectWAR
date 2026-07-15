cmake_minimum_required(VERSION 3.10)
project(WarZone)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3 -fPIC")

add_library(WarZone SHARED
    Source.cpp
    Intersections.cpp
    KDTreeCPU.cpp
    KDTreeNode.cpp
    ZoneManager.cpp
    Util.cpp
)

# Use system glm header but do NOT include the local subdirectory to avoid redefinitions
find_package(glm REQUIRED)
target_link_libraries(WarZone PRIVATE glm::glm)
