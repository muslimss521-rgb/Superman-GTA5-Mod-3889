private Entity GetTarget(Ped player, float distance)
{
    Ped[] peds = World.GetNearbyPeds(player, distance);

    Ped best = null;
    float bestDistance = distance;

    foreach (Ped ped in peds)
    {
        if (ped == null || !ped.Exists() || ped.Handle == player.Handle)
        {
            continue;
        }

        Vector3 direction = ped.Position - player.Position;
        float currentDistance = direction.Length();

        if (currentDistance > distance || currentDistance <= 0.1f)
        {
            continue;
        }

        direction.Normalize();

        float dot =
            Vector3.Dot(player.ForwardVector, direction);

        if (dot < 0.65f)
        {
            continue;
        }

        if (currentDistance < bestDistance)
        {
            best = ped;
            bestDistance = currentDistance;
        }
    }

    return best;
}
