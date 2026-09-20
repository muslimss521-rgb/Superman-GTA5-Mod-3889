#pragma once

#include <cstdint>

namespace SupermanNative
{
    struct Vector3
    {
        float x;
        float y;
        float z;
    };

    struct FlightSettings
    {
        float hoverSpeed = 8.0f;
        float flightSpeed = 45.0f;
        float boostSpeed = 110.0f;
        float supersonicSpeed = 220.0f;

        float acceleration = 35.0f;
        float deceleration = 28.0f;
        float turnResponsiveness = 2.2f;
        float bodyLean = 18.0f;
    };

    struct PhysicsSettings
    {
        float punchForce = 18.0f;
        float heavyPunchForce = 45.0f;
        float chargedPunchForce = 85.0f;

        float vehicleForce = 70.0f;
        float throwForce = 95.0f;
        float groundPoundForce = 120.0f;

        float shockwaveRadius = 12.0f;
    };

    enum class SupermanState
    {
        Normal,
        TakingOff,
        Hover,
        Flight,
        Boost,
        Supersonic,
        Braking,
        Landing,
        Combat,
        Grab,
        Carry,
        Throw
    };

    enum class SupermanPower
    {
        None,
        SuperSpeed,
        HeatVision,
        LaserBlast,
        FreezeBreath,
        SuperBreath,
        SuperJump,
        GroundPound,
        ThunderClap,
        Tornado
