﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility;

namespace Pilgrimage;

public interface IQuestService
{
    Task<Result> LoadQuests(IEnumerable<string> filenames);

    Task<Result> LoadQuest(string filename);

    Task<Result> AddQuest(Quest quest);

    bool HasLoaded { get; }

    /// <summary>Raised when a quest's state has changed. e.g. quest has completed</summary>
    event EventHandler<QuestStateChangedEventArgs> QuestStateChanged;

    event EventHandler<QuestLoadedEventArgs> QuestLoaded;

    event EventHandler QuestsLoaded;

    /// <summary>Get the list of available quests for the player that can be started.</summary>
    /// <param name="player">The player.</param>
    /// <returns>Returns a list of available quests.</returns>
    /// <remarks>
    /// This will not return a quest that has been started.
    /// This will return quests that have completed the pre-reqs, otherwise they will be excluded.
    /// </remarks>
    Result<IEnumerable<Quest>> GetAvailableQuests(PilgrimPlayer player);

    Result<IEnumerable<Quest>> GetInProgressQuests(PilgrimPlayer player);

    /// <summary>Get the entire list of quests.</summary>
    /// <returns>Returns a list of all the loaded quests.</returns>
    IEnumerable<Quest> GetAllQuests();

    Task<Result<bool>> IsPreRequisite(int questId, int preReqQuestId);

    Task<Result> IsItemRequiredByActiveQuest(PilgrimPlayer player, int itemId);

    Task<Result<Quest>> GetQuestForItem(int itemId);

    Task<Result> HasStartedQuest(PilgrimPlayer player, int questId);

    Task<Result> CanStartQuest(PilgrimPlayer player, int questId);

    Task<Result> CanCompleteQuest(PilgrimPlayer player, int questId);

    Task<Result> HasCompletedQuest(PilgrimPlayer player, int questId);

    Task<Result> StartQuest(PilgrimPlayer player, int id);

    Task<Result> CompleteQuest(PilgrimPlayer player, IItemService items, IInventoryService inventory, int id);
}
