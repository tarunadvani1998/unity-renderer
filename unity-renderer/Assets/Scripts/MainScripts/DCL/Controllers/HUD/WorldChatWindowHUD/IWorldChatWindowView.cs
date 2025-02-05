using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldChatWindowView
{
    event Action OnClose;
    event Action<string> OnOpenPrivateChat;
    event Action<string> OnOpenPublicChannel;
    event Action<string> OnUnfriend;
    event Action<string> OnSearchChannelRequested;
    event Action OnRequireMorePrivateChats;
    
    RectTransform Transform { get; }
    bool IsActive { get; }
    int PrivateChannelsCount { get; }

    void Initialize(IChatController chatController, ILastReadMessagesService lastReadMessagesService);
    void Show();
    void Hide();
    void SetPrivateChat(PrivateChatModel model);
    void RemovePrivateChat(string userId);
    void SetPublicChannel(PublicChatChannelModel model);
    void ShowPrivateChatsLoading();
    void HidePrivateChatsLoading();
    void RefreshBlockedDirectMessages(List<string> blockedUsers);
    void Dispose();
    void ClearFilter();
    void HideMoreChatsToLoadHint();
    void ShowMoreChatsToLoadHint(int count);
    void Filter(Dictionary<string,PrivateChatModel> privateChats, Dictionary<string,PublicChatChannelModel> publicChannels);
    bool ContainsPrivateChannel(string userId);
}