using Content.Server.Forensics;
using Content.Server.Station.Systems;
using Content.Server.StationRecords.Systems;
using Content.Shared._MadStation.Cvars;
using Content.Shared.Mind;
using Content.Shared.Pulling.Components;
using Content.Shared.Roles.Jobs;
using Content.Shared.SSDIndicator;
using Content.Shared.StationRecords;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server._MadStation;

public sealed class SsdTeleportationSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedJobSystem _jobSystem = default!;
    [Dependency] private readonly SharedMindSystem _mindSystem = default!;
    [Dependency] private readonly StationJobsSystem _stationJobsSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly StationRecordsSystem _recordsSystem = default!;


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
                TryFreeJob(uid);
                QueSsdDeletion(uid);
            }
        }
    }

    private void TryFreeJob(EntityUid uid)
    {
        if(!TryComp<DnaComponent>(uid, out var dna))
        {
            return;
        }

        if (!_mindSystem.TryGetMind(uid, out var _, out var _))
        {
            return;
        }

        if (!_jobSystem.MindTryGetJob(uid, out var _, out var jobPrototype))
        {
            return;
        }

        var stations = _stationSystem.GetStations();

        foreach (var station in stations)
        {
            var records = _recordsSystem.GetRecordsOfType<GeneralStationRecord>(station);
            foreach (var (_, record) in records)
            {
                if (record.DNA == dna.DNA)
                {
                    _stationJobsSystem.TryAdjustJobSlot(station, jobPrototype, 1);
                }
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
