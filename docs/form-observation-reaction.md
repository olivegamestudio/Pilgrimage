# The Form, Observation, and Reaction Model

## Status

This document records the current conceptual direction for Pilgrimage and its
possible lower-level foundation. It is not a settled API specification. Names
and boundaries must be proved against several different game behaviours before
they are committed to code.

The current implementation begins at the quest layer:

```text
Player -> QuestLedger -> QuestProgress
Quest  -> Requirements + Objectives + Rewards
```

That model is a useful prototype, but it assumes too much: a single player,
explicit quest start and completion, player-state predicates, linear progress,
and rewards as the principal consequence. The more general behaviour lies
underneath those concepts.

## Core proposition

A game world is a habitat of composed Forms that selectively observe and
react.

```text
Observation -> Form -> Reaction -> Observation
```

A Form's composition determines:

- what it can observe;
- which observations interest it;
- what it can remember;
- how it can react;
- what it can emit; and
- how it can appear.

Concepts such as quests, objectives, players, ships, storms, achievements,
physics, input mappings, and narrative beats are higher-level compositions of
this behaviour. They are not assumed by the foundation.

## Language

The vocabulary should remain deliberately small. A type should be introduced
only when it protects a real semantic distinction.

### Form

A Form is an identified composition with retained state and the capacity to
observe and react.

```text
Form
|- identity
|- composition
|- retained state
|- interests
|- possible reactions
|- possible emissions
`- visual phenotype
```

Form describes the makeup of anything that exists in the game world. A ship,
player, quest instance, faction, storm, sector, or world process can all be
Forms because of their different compositions, not because they inherit from a
universal game-object hierarchy.

A Form is not necessarily an independently hosted service. A composition
becomes independently addressable only when its identity, state, lifecycle,
concurrency, or observation pattern requires that independence. A simple
objective may remain part of a quest Form; a complex, long-lived objective may
become its own Form.

### Composition

Composition is the Form's makeup. Its building blocks contribute some
combination of:

- observation interests;
- retained state;
- reaction semantics;
- possible emissions;
- persistence requirements;
- visual expression; and
- explanation and provenance.

This resembles ECS in its preference for composition over inheritance, but it
does not yet require an ECS implementation. The building blocks are not merely
passive data operated on by global systems; they can be stateful behavioural
capabilities.

For example:

```text
Defend Echelon
|- undertaking behaviour
|- squad participation
|- prerequisite-completion interest
|- ship-destruction evidence accumulator
|- deadline behaviour
|- success and failure reactions
`- contribution-based consequence
```

### Observation

An Observation presents something as having happened or become perceivable to
a Form. Its value should retain the game's own language:

```text
Observation<ShipDestroyed>
|- identity
|- ShipDestroyed value
|- source and authority
|- scope
|- occurred and observed time
|- causation
`- correlation
```

The foundation must not erase meaning by translating this into a generic
property bag such as `EntityChanged`. Battle Force owns terms such as
`ShipDestroyed`, `SectorCaptured`, and `SquadFormed`. Another game owns a
different vocabulary.

"Occurrence", "emission", and "observation" describe distinguishable moments
in ordinary language, but they may overlap in code. The initial API should not
create separate wrapper types without a demonstrated need. `Emit` and `Observe`
may remain verbs applied to the same immutable `Observation<T>`.

### Interest

An Interest is a declarative registration describing observations that could
matter to a Form.

An Interest is not an opaque predicate or lambda. Its structure needs to be
inspectable so that it can be:

- indexed for selective routing;
- displayed as part of a Form's makeup;
- persisted and versioned;
- explained to a designer;
- generated or learned safely; and
- compared with other interests.

For example:

```text
Interested in: UndertakingCompleted
Definition:    First Contact
Scope:         same participant
Authority:     authoritative game world
Lifecycle:     until this Form becomes eligible
```

An interest controls attention, not meaning. Routing can safely produce false
positives, which a Form may ignore. It must not produce false negatives by
encoding game rules it does not understand.

### Reaction

A Reaction describes the transformation produced when a Form interprets an
Observation.

```text
Reaction
|- no change
|- retained-state changes
|- composition changes
|- interest registrations or removals
|- new Forms
|- dissolved Forms
`- Observations to emit
```

Where practical, a Reaction should be data describing a transformation rather
than opaque infrastructure side effects. This makes it testable, explainable,
persistable, replayable, visualisable, and host-independent.

### Habitat

A Habitat sustains Forms and routes Observations according to declared
Interests. It provides the conditions in which Forms can appear logically
alive, but it does not own their game-specific rules.

The Habitat may eventually provide identity, activation, delivery,
persistence, scheduling, and placement. Those responsibilities do not require
the core domain model to commit to a particular distributed runtime.

## Autonomous observers

Every living quest instance is an autonomous observer with its own rules. It
listens for Observations relevant to its prerequisites and determines for
itself whether it can transition.

When quest A completes, it does not activate B or C and does not know that they
exist:

```text
Quest A completes
       |
       `- emits UndertakingCompleted(A)
                         |
                         `- Habitat resolves registered interests
                                      |- Quest B observes and reacts
                                      `- Quest C observes and reacts
```

The entirety of the quest catalogue is not informed. B and C declared an
interest in A's completion, so only plausible candidates receive it. Each then
uses its own composition and retained state to determine its reaction.

The division of responsibility is:

- A owns its completion and emits an immutable fact.
- The Habitat owns selective delivery, not quest semantics.
- B and C own their interpretation and transitions.

An inactive definition is inert knowledge, not a permanently active service.
Only a living instance has state and interests. A virtual-actor runtime may make
an instance logically addressable while leaving it physically dormant until a
relevant Observation arrives.

## Pilgrimage's layer

Pilgrimage sits above the neutral Form/Observation/Reaction foundation. It
provides an undertaking-oriented composition language.

```text
Game-specific language
Quest, mission, contract, challenge, campaign
                  |
Pilgrimage
Participation, eligibility, evidence, outcomes, consequences
                  |
Reactive foundation
Forms, composition, Interests, Observations, Reactions, Habitat
```

Pilgrimage concepts can be understood as specialised compositions:

| Pilgrimage concept | Underlying behaviour |
| --- | --- |
| Requirement | Interest whose retained interpretation contributes to eligibility |
| Objective | Evidence-oriented composition that contributes to an outcome |
| Progress | A projection of retained interpretation, not necessarily source truth |
| Completion | One possible terminal transition |
| Reward | One possible consequence |
| Prerequisite | Interest in another undertaking's transition |

Different games can present the same broad undertaking pattern as a quest,
mission, contract, case, challenge, or directive while supplying different
participation, evidence, temporal, and outcome rules.

The layer should not grow into a universal `Quest` class containing flags for
every possible game. Game-specific rules should form through composition.

## Actors and scope

Domain actors include players, squads, guilds, factions, NPCs, worlds, and
temporary teams. They are not identical to actor-model runtime actors.

Observations and Interests need scope so that completion by one participant
does not wake instances belonging to every participant. Relevant scopes can
include:

- player;
- squad;
- faction;
- battle;
- sector;
- campaign; and
- world.

One logical Form may represent a player, but a mission or sector may also be a
Form despite not being a participant. Runtime actor boundaries should follow
independent identity, consistency, lifecycle, and scaling needs rather than
domain nouns.

## Evidence and saving state

Progress is not generally source truth. It is an interpretation of evidence
under a particular version of the rules.

Saving `37 of 50 ships destroyed` alone loses:

- which Observations counted;
- whether evidence was duplicated;
- its source and authority;
- participant contributions;
- the applicable time window;
- the rule version used; and
- the reason the state is believed to be correct.

A durable process may need to preserve:

```text
Evidence
    +
Interpretation and rule version
    +
Current projection or snapshot
```

The exact retention strategy may vary by Form. Valuable cloud progression may
need retained evidence plus snapshots and checkpoints. Lower-value behaviour
may need only derived state.

Delivery should normally assume retries. Forms and their consequences need
durable Observation identities, idempotent reactions, atomic local state
transitions, and recoverable pending emissions. A completion that changes state
but fails before emitting its consequence must be able to resume safely.

## Selective routing

A global broadcast would wake thousands of irrelevant Forms. Interests should
therefore be compiled into structural routing indexes.

```text
Observation<ShipDestroyed>
|- battle/308
|- sector/echelon
|- player/42
|- squad/7
`- faction/corsair
```

The Habitat uses type and scope to obtain a small candidate set. Candidate
Forms retain the authority to interpret the Observation.

Rules declare which changes could affect them. For example, a quest requiring
completion of A and sufficient reputation declares interests in completion of
A and relevant reputation changes. It does not repeatedly inspect a complete
Player object through `IsMetBy(Player)`.

Interest registrations should follow lifecycle. When an interest can no
longer affect a Form, it should be removed. Shared world conditions may be
represented by one shared Form rather than thousands of identical personal
observers.

## Evolving worlds and simulation resolution

Battle Force is an evolving world, not a static set of levels. Logical
persistence does not require everything to execute continuously.

Forms can remain dormant until an Observation, request, or scheduled interest
wakes them. The world can also operate at different resolutions:

```text
Active battle       individual ships and high-frequency action
Active sector       fleets, resources, control, and undertakings
Dormant region      aggregate state and scheduled developments
Distant world       compressed state and long-term processes
```

Consequences should propagate at increasing abstraction. A ship destruction
affects its battle; a battle outcome may affect a fleet; a fleet change may
affect a sector; sector capture may affect a campaign. Low-level observations
should not wake the entire world.

## Visual phenotype and the camera

A Form's composition determines its visual phenotype. Visuals are not arbitrary
decoration attached to unrelated data; they expose what the Form is made of and
its current state.

```text
Makeup                         Possible visual expression
receptors                      sensitive membrane
retained evidence              internal bodies or rings
accumulator                    filling chamber
deadline                       changing or contracting boundary
multiple participants          linked nuclei
pending emission               activity at the membrane
damage                         disrupted structure
```

The renderer is a camera over how Forms look:

```text
Form phenotype + Camera lens -> rendered appearance
```

The camera does not own the meaning of a ship, quest, or player. It selects a
scope and projects perceptible aspects of Forms. Different cameras can project
the same underlying Forms differently:

- a gameplay camera presents the player-facing world;
- a microscope camera reveals composition and retained state;
- a causal camera reveals Observation propagation;
- a diagnostic camera reveals authority and dormant Interests; and
- a historical camera reveals change over time.

The camera can itself be modelled as a Form with scope, projection, visual
interests, and an output surface. This conceptual consistency does not require
high-frequency rendering work to become distributed messages; an implementation
may batch and optimise local projection.

Input can be understood as the inverse projection. The camera maps a player's
contact with a visible representation back into a scoped, game-specific
Observation. The receiving Form determines its reaction.

## The Petri dish

The Petri dish is not a flowchart or node editor. It is an instrumented living
simulation in which design and runtime occupy the same environment.

The designer should manipulate behaviour directly:

1. place or select a Form;
2. stimulate it with an Observation;
3. observe its Reaction;
4. demonstrate the desired response;
5. accept or correct the developed composition; and
6. test the behaviour in new situations.

Persistent wires and implementation graphs should not dominate the interface.
Signals can appear temporarily as pulses or causal trails. A Form should reveal
its makeup only when inspected.

Clicking a blob opens a microscope over its genetics:

- inherited and game-specific composition;
- registered Interests;
- possible Reactions;
- retained evidence and current state;
- possible emissions;
- definition and instance versions; and
- provenance of learned, generated, inherited, and corrected traits.

Two instances can share a genotype while expressing different lived states.
The outside of the blob is its phenotype; the microscope explains the makeup
that permits that behaviour.

## Development observation and automation

Building the game is itself observable. The environment can observe meaningful
development changes such as introducing a Form, demonstrating a capability,
accepting a proposed trait, rejecting a Reaction, establishing an invariant,
or passing a scenario.

Automation is not playback of clicks and drags:

```text
Playback   = repeat recorded surface actions
Automation = observe state, choose an action, verify, and adapt
```

An automated participant should operate on intent, Form composition, observed
behaviour, and evidence. It can propose missing traits, create experiments,
request teaching where intent is ambiguous, and verify whether a proposed
composition generalises.

Game completion cannot be predicted reliably from completed task counts. It is
a game-specific confidence judgement based on demonstrated behaviour, stable
interactions, end-to-end player journeys, preserved invariants, unresolved
uncertainty, and release constraints.

## Orleans as a possible Habitat

Orleans is a possible distributed host, not the definition of the domain.
Virtual actors align with long-lived, mostly dormant Forms because they provide
logical identity, activation, state, and placement without requiring every Form
to remain physically running.

A possible mapping is:

| Concept | Orleans mechanism |
| --- | --- |
| independently living Form | grain |
| Form identity | grain key |
| retained state | grain state |
| relevant delivery | direct message or stream |
| temporal interest | timer or reminder |
| habitat placement | silo runtime |

Pilgrimage's fundamental types should not depend directly on Orleans grain
interfaces. A pure transition model should be testable in memory, with an
optional Orleans adapter hosting Forms that genuinely need distributed
identity, persistence, and concurrency.

Orleans does not decide evidence semantics, authority, idempotency, event
retention, or rule versioning. Those remain responsibilities of the domain and
the Habitat contract.

## Layering

The current direction can be summarised as:

```text
Game-specific language
Battle Force ships, sectors, missions, squads, and world rules
                         |
Pilgrimage
Undertaking-oriented compositions and interpretations
                         |
Reactive foundation
Form, Composition, Interest, Observation, Reaction, Habitat
                         |
Optional hosts
In-memory, local runtime, Orleans, or another execution environment
                         |
Infrastructure
Storage, scheduling, networking, rendering surfaces, and devices
```

No higher layer should force its vocabulary into the layer beneath it.

## Questions that remain deliberately open

The following should be resolved through examples and experiments rather than
terminology alone:

1. Does `Observation<T>` represent a world fact, a source's claim, or one
   receiver's perception?
2. Does an Interest contain matching semantics, or only enough structure for
   safe candidate routing?
3. Which Reactions can be represented as data, and where is executable
   behaviour unavoidable?
4. What is the smallest independently living Form?
5. When should a composition remain internal, and when should it become its own
   Form?
6. Which evidence must be retained, and which projections can be trusted as
   sufficient state?
7. How are composition and rule versions applied to already living Forms?
8. How are authoritative time, absence, continuous conditions, and revocation
   represented?
9. Which parts of visual phenotype belong to a Form and which belong to a
   Camera lens?
10. Can the same foundation naturally model a quest dependency, player input,
    ship movement, a timed storm, an achievement, and a development
    demonstration without special cases?

The final question is the principal test of the abstraction. Familiar game
concepts should be named only after they have emerged as useful compositions of
observation and reaction.

