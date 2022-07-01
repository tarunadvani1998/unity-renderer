using DCL.Interface;
using System;
using System.Collections.Generic;

public interface IChatController
{
    int TotalUnseenMessages { get; }
    
    event Action<ChatMessage> OnAddMessage;
    event Action<int> OnTotalUnseenMessagesUpdated;
    event Action<string, int> OnUserUnseenMessagesUpdated;

    List<ChatMessage> GetAllocatedEntries();
    List<ChatMessage> GetPrivateAllocatedEntriesByUser(string userId);
    void Send(ChatMessage message);
    void MarkMessagesAsSeen(string userId);
    void GetPrivateMessages(string userId, int limit, long fromTimestamp);
    int GetUnseenMessagesCount(string userId);
}