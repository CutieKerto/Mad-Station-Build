using Content.Server.Access.Components;
using Content.Server.GameTicking;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Shared.Access.Systems;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;

namespace Content.Server.Access.Systems;

public sealed class PresetIdCardSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IdCardSystem _cardSystem = default!;
    [Dependency] private readonly SharedAccessSystem _accessSystem = default!;
    [Dependency] private readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly SharedJobSystem _jobSystem = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<PresetIdCardComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(PlayerJobsAssigned);
    }

    private void PlayerJobsAssigned(RulePlayerJobsAssignedEvent ev)
    {
        // Go over all ID cards and make sure they're correctly configured for extended access.

        var query = EntityQueryEnumerator<PresetIdCardComponent>();
        while (query.MoveNext(out var uid, out var card))
        {
            var station = _stationSystem.GetOwningStation(uid);

            // If we're not on an extended access station, the ID is already configured correctly from MapInit.
            if (station == null || !Comp<StationJobsComponent>(station.Value).ExtendedAccess)
                return;

            SetupIdAccess(uid, card, true);
            SetupIdName(uid, card);
			SetupIdColor(uid, card);
        }
    }

    private void OnMapInit(EntityUid uid, PresetIdCardComponent id, MapInitEvent args)
    {
        // If a preset ID card is spawned on a station at setup time,
        // the station may not exist,
        // or may not yet know whether it is on extended access (players not spawned yet).
        // PlayerJobsAssigned makes sure extended access is configured correctly in that case.

        var station = _stationSystem.GetOwningStation(uid);
        var extended = false;
        if (station != null)
            extended = Comp<StationJobsComponent>(station.Value).ExtendedAccess;

        SetupIdAccess(uid, id, extended);
        SetupIdName(uid, id);
    }

    private void SetupIdName(EntityUid uid, PresetIdCardComponent id)
    {
        if (id.IdName == null)
            return;
        _cardSystem.TryChangeFullName(uid, id.IdName);
    }
	
	private void SetupIdColor(EntityUid uid, PresetIdCardComponent id)
    {
        if (id.JobName is not null)
		{
			var color = _prototypeManager.TryIndex(id.JobName, out JobPrototype? job) && job.Color is not null ? job.Color
						: _jobSystem.TryGetDepartment(id.JobName, out var department) ? department.Color 
						: Color.FromHex("#FFFFFF");
			
			_cardSystem.TryChangeColor(uid, color);
		}
    }

    private void SetupIdAccess(EntityUid uid, PresetIdCardComponent id, bool extended)
    {
        if (id.JobName == null)
            return;

        if (!_prototypeManager.TryIndex(id.JobName, out JobPrototype? job))
        {
            Log.Error($"Invalid job id ({id.JobName}) for preset card");
            return;
        }

        _accessSystem.SetAccessToJob(uid, job, extended);

        _cardSystem.TryChangeJobTitle(uid, job.LocalizedName);
        _cardSystem.TryChangeJobDepartment(uid, job);

        if (_prototypeManager.TryIndex<StatusIconPrototype>(job.Icon, out var jobIcon))
        {
            _cardSystem.TryChangeJobIcon(uid, jobIcon);
        }
    }
}
