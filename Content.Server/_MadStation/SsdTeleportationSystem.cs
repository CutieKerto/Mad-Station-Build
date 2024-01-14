using Content.Shared._MadStation.Cvars;
using Content.Shared.Anomaly.Components;
using Content.Shared.Mind;
using Content.Shared.Pulling.Components;
using Content.Shared.Roles.Jobs;
using Content.Shared.Sound.Components;
using Content.Shared.SSDIndicator;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Server._MadStation;

public sealed class SsdTeleportationSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedJobSystem _jobSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly ITimerManager _timerManager = default!;




    private float _maxSsdTime = default!;
    private bool _enabled = default!;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(MadCvars.MaxSsdTime, newValue => _maxSsdTime = newValue, true);
        _cfg.OnValueChanged(MadCvars.SsdTeleportationEnabled, newValue => _enabled = newValue, true);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        if (!_enabled)
        {
            return;
        }

        var query = EntityQueryEnumerator<SSDIndicatorComponent>();

        while (query.MoveNext(out var uid, out var ssdIndicator))
        {
            if (!ssdIndicator.IsSSD)
            {
                continue;
            }

            if (ssdIndicator.IsTeleporting)
            {
                continue;
            }

            var delta = (DateTime.Now - ssdIndicator.SsdStartTime).TotalMinutes;

            if (delta > _maxSsdTime)
            {
                var position = Transform(uid);
                var portal = _random.Pick(SSDIndicatorComponent.Portals);

                Spawn(portal, position.Coordinates);

                RemComp<SharedPullableComponent>(uid);

                ssdIndicator.IsTeleporting = true;

                QueSsdDeletion(uid);
            }
        }

    }

    private void QueSsdDeletion(EntityUid entityToDelete)
    {
        Timer.Spawn(TimeSpan.FromSeconds(2.5f), () =>
        {
            _audioSystem.PlayPvs(SSDIndicatorComponent.DepartureSound, Transform(entityToDelete).Coordinates);
            Del(entityToDelete);
        });
    }
}
