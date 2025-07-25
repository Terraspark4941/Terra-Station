// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2024 MilenVolf <63782763+MilenVolf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 ShadowCommander <10494922+ShadowCommander@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Access.Components;
using Content.Shared.DoAfter;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Lock;

/// <summary>
/// Allows locking/unlocking, with access determined by AccessReader
/// </summary>
[RegisterComponent, NetworkedComponent]
[Access(typeof(LockSystem))]
[AutoGenerateComponentState]
public sealed partial class LockComponent : Component
{
    /// <summary>
    /// Whether or not the lock is locked.
    /// </summary>
    [DataField("locked"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool Locked  = true;

    /// <summary>
    /// Whether or not the lock is locked by simply clicking.
    /// </summary>
    [DataField("lockOnClick"), ViewVariables(VVAccess.ReadWrite)]
    [AutoNetworkedField]
    public bool LockOnClick;

    /// <summary>
    /// Whether or not the lock is unlocked by simply clicking.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool UnlockOnClick = true;

    /// <summary>
    /// Whether the lock requires access validation through <see cref="AccessReaderComponent"/>
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool UseAccess = true;

    /// <summary>
    /// The sound played when unlocked.
    /// </summary>
    [DataField("unlockingSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier UnlockSound = new SoundPathSpecifier("/Audio/Machines/door_lock_off.ogg")
    {
        Params = AudioParams.Default.WithVolume(-5f),
    };

    /// <summary>
    /// The sound played when locked.
    /// </summary>
    [DataField("lockingSound"), ViewVariables(VVAccess.ReadWrite)]
    public SoundSpecifier LockSound = new SoundPathSpecifier("/Audio/Machines/door_lock_on.ogg")
    {
        Params = AudioParams.Default.WithVolume(-5f)
    };

    /// <summary>
    /// Whether or not an emag disables it.
    /// </summary>
    [DataField]
    [AutoNetworkedField]
    public bool BreakOnAccessBreaker = true;

    /// <summary>
    /// Amount of do-after time needed to lock the entity.
    /// </summary>
    /// <remarks>
    /// If set to zero, no do-after will be used.
    /// </remarks>
    [DataField]
    [AutoNetworkedField]
    public TimeSpan LockTime;

    /// <summary>
    /// Amount of do-after time needed to unlock the entity.
    /// </summary>
    /// <remarks>
    /// If set to zero, no do-after will be used.
    /// </remarks>
    [DataField]
    [AutoNetworkedField]
    public TimeSpan UnlockTime;
}

/// <summary>
/// Event raised on the lock when a toggle is attempted.
/// Can be cancelled to prevent it.
/// </summary>
[ByRefEvent]
public record struct LockToggleAttemptEvent(EntityUid User, bool Silent = false, bool Cancelled = false);

/// <summary>
/// Event raised on the user when a toggle is attempted.
/// Can be cancelled to prevent it.
/// </summary>
[ByRefEvent]
public record struct UserLockToggleAttemptEvent(EntityUid Target, bool Silent = false, bool Cancelled = false);

/// <summary>
/// Event raised on a lock after it has been toggled.
/// </summary>
[ByRefEvent]
public readonly record struct LockToggledEvent(bool Locked);

/// <summary>
/// Used to lock a lockable entity that has a lock time configured.
/// </summary>
/// <seealso cref="LockComponent"/>
/// <seealso cref="LockSystem"/>
[Serializable, NetSerializable]
public sealed partial class LockDoAfter : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}

/// <summary>
/// Used to unlock a lockable entity that has an unlock time configured.
/// </summary>
/// <seealso cref="LockComponent"/>
/// <seealso cref="LockSystem"/>
[Serializable, NetSerializable]
public sealed partial class UnlockDoAfter : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}

[NetSerializable]
[Serializable]
public enum LockVisuals : byte
{
    Locked
}