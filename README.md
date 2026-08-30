# Pilgrimage

Pilgrimage is exploring how game-specific concepts such as quests, missions,
objectives, achievements, campaigns, and evolving world processes can form from
a smaller reactive foundation.

Pilgrimage builds on [Forma](https://github.com/retroreborn/FORMA). During local
development the project references the sibling `FORMA` checkout directly.

The current source is an early quest-shaped prototype. It is useful evidence,
but types such as `Quest`, `Player`, `Objective`, and `QuestProgress` are not yet
assumed to be the foundational abstraction.

The emerging model is documented in
[The Form, Observation, and Reaction Model](docs/form-observation-reaction.md).
It is intentionally a conceptual foundation rather than an implementation plan
or a frozen public API.

Each living `Quest` is a Forma `Form`. Its requirements, objectives, rewards,
and repeat policy are Forma traits; its status and prerequisite evidence are
retained in `QuestInstanceState`. Completing a quest emits a typed observation,
and `QuestSet` routes that observation only to quests declaring a matching
interest. `Session` currently acts as the small in-memory host for this model.
