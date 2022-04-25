using System;
using System.Linq;
using DCL;
using DCL.Interface;

public class PublicChatChannelController : IHUD
{
    public IChannelChatWindowView view;
    public event Action OnBack;
    public event Action OnClosed;

    private readonly IChatController chatController;
    private readonly ILastReadMessagesService lastReadMessagesService;
    private readonly IUserProfileBridge userProfileBridge;
    private readonly InputAction_Trigger closeWindowTrigger;
    private ChatHUDController chatHudController;

    internal string lastPrivateMessageRecipient = string.Empty;
    private double initTimeInSeconds;
    private string channelId;

    private UserProfile ownProfile => UserProfile.GetOwnUserProfile();

    public PublicChatChannelController(IChatController chatController,
        ILastReadMessagesService lastReadMessagesService,
        IUserProfileBridge userProfileBridge,
        InputAction_Trigger closeWindowTrigger)
    {
        this.chatController = chatController;
        this.lastReadMessagesService = lastReadMessagesService;
        this.userProfileBridge = userProfileBridge;
        this.closeWindowTrigger = closeWindowTrigger;
    }

    public void Initialize(IChannelChatWindowView view = null)
    {
        view ??= PublicChatChannelComponentView.Create();
        this.view = view;
        view.OnClose += HandleViewClosed;
        view.OnBack += HandleViewBacked;
        closeWindowTrigger.OnTriggered -= HandleCloseInputTriggered;
        closeWindowTrigger.OnTriggered += HandleCloseInputTriggered;

        chatHudController = new ChatHUDController(DataStore.i,
            new UserProfileWebInterfaceBridge(),
            true,
            ProfanityFilterSharedInstances.regexFilter);
        chatHudController.Initialize(view.ChatHUD);
        chatHudController.OnSendMessage += SendChatMessage;
        chatHudController.OnMessageUpdated += HandleMessageInputUpdated;
        chatHudController.OnInputFieldSelected += HandleInputFieldSelected;

        if (chatController != null)
        {
            chatController.OnAddMessage -= HandleMessageReceived;
            chatController.OnAddMessage += HandleMessageReceived;
        }
        
        initTimeInSeconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
    }

    public void Setup(string channelId)
    {
        this.channelId = channelId;
        
        // TODO: retrieve data from a channel provider
        view.Setup(this.channelId, "General", "Any useful description here");
        
        chatHudController.ClearAllEntries();
        var messageEntries = chatController.GetEntries()
            .ToList();
        foreach (var v in messageEntries)
            HandleMessageReceived(v);
    }

    public void Dispose()
    {
        view.OnClose -= HandleViewClosed;
        view.OnBack -= HandleViewBacked;

        if (chatController != null)
            chatController.OnAddMessage -= HandleMessageReceived;

        chatHudController.OnSendMessage -= SendChatMessage;
        chatHudController.OnMessageUpdated -= HandleMessageInputUpdated;

        view?.Dispose();
    }

    public void SendChatMessage(ChatMessage message)
    {
        bool isValidMessage = !string.IsNullOrEmpty(message.body) && !string.IsNullOrWhiteSpace(message.body);
        bool isPrivateMessage = message.messageType == ChatMessage.Type.PRIVATE;
        
        if (isPrivateMessage && isValidMessage)
            lastPrivateMessageRecipient = message.recipient;
        else
            lastPrivateMessageRecipient = null;

        if (!isValidMessage)
        {
            chatHudController.ResetInputField(true);
            return;
        }

        chatHudController.ResetInputField();
        chatHudController.FocusInputField();

        if (isPrivateMessage)
            message.body = $"/w {message.recipient} {message.body}";

        chatController.Send(message);
    }

    public void SetVisibility(bool visible)
    {
        if (visible)
        {
            view.Show();
            MarkChatMessagesAsRead();
            chatHudController.FocusInputField();
        }
        else
            view.Hide();
    }

    private void MarkChatMessagesAsRead() => lastReadMessagesService.MarkAllRead(channelId);

    public void ResetInputField() => chatHudController.ResetInputField();

    private void HandleCloseInputTriggered(DCLAction_Trigger action) => HandleViewClosed();

    private void HandleViewClosed() => OnClosed?.Invoke();

    private void HandleViewBacked() => OnBack?.Invoke();

    private void HandleMessageInputUpdated(string message)
    {
        if (!string.IsNullOrEmpty(lastPrivateMessageRecipient) && message == "/r ")
            chatHudController.SetInputFieldText($"/w {lastPrivateMessageRecipient} ");
    }
    
    private void HandleInputFieldSelected()
    {
        if (string.IsNullOrEmpty(lastPrivateMessageRecipient)) return;
        chatHudController.SetInputFieldText($"/w {lastPrivateMessageRecipient} ");
    }

    private bool IsOldPrivateMessage(ChatMessage message)
    {
        if (message.messageType != ChatMessage.Type.PRIVATE) return false;
        var timestampInSeconds = message.timestamp / 1000.0;
        return timestampInSeconds < initTimeInSeconds;
    }

    private void HandleMessageReceived(ChatMessage message)
    {
        if (IsOldPrivateMessage(message)) return;

        chatHudController.AddChatMessage(message, view.IsActive);

        if (message.messageType == ChatMessage.Type.PRIVATE && message.recipient == ownProfile.userId)
            lastPrivateMessageRecipient = userProfileBridge.Get(message.sender).userName;
    }
}