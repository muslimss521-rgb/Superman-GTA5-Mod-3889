#include "SupermanNative.h"

namespace SupermanNative
{
    void SupermanNative::Initialize()
    {
        if (m_initialized)
            return;

        m_initialized = true;
        m_supermanEnabled = false;
        m_flightEnabled = false;
        m_superSpeedEnabled = false;

        m_state = SupermanState::Normal;
        m_power = SupermanPower::None;
    }

    void SupermanNative::Update(float deltaTime)
    {
        if (!m_initialized)
            return;

        if (deltaTime <= 0.0f)
            return;

        // Основная логика GTA V будет подключена
        // на следующем этапе.
        //
        // Здесь будут:
        // - управление полётом
        // - ускор
