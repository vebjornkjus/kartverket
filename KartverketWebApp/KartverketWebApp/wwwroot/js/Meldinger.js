// Constants
const DOM_SELECTORS = {
    conversationContainer: "#conversation-container",
    messageForm: "#sendMessageForm",
    messageInput: "#messageInput",
    activeConversationRapportId: "#activeConversationRapportId",
    activeConversationRecipientId: "#activeConversationRecipientId",
    conversationCards: ".conversation-card"
};

// Message Service - Handles all API calls
class MessageService {
    static async fetchConversation(rapportId) {
        const response = await fetch(`/Meldinger/GetConversation?rapportId=${rapportId}`);
        if (!response.ok) {
            throw new Error(`Failed to fetch messages: ${response.statusText}`);
        }
        return await response.json();
    }

    static async sendMessage(formData) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const response = await fetch('/Meldinger/SendMessage', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token
            },
            body: formData
        });
        return await response.json();
    }
}

// UI Manager - Handles DOM manipulation and UI updates
class ChatUIManager {
    constructor() {
        this.container = document.querySelector(DOM_SELECTORS.conversationContainer);
    }

    renderMessages(messages) {
        if (!this.container) {
            throw new Error("Conversation container not found in the DOM.");
        }

        this.container.innerHTML = "";
        messages.forEach(msg => this.renderMessage(msg));
        this.scrollToBottom();
    }

    renderMessage(msg) {
        const msgElement = document.createElement("div");
        msgElement.className = msg.isSender ? "message-sent" : "message-received";
        msgElement.innerHTML = `
            <p>${msg.innhold}</p>
            <span class="timestamp">${msg.tidsstempel}</span>
        `;
        this.container.appendChild(msgElement);
    }

    scrollToBottom() {
        if (this.container) {
            this.container.scrollTop = this.container.scrollHeight;
        }
    }

    clearMessageInput() {
        document.querySelector(DOM_SELECTORS.messageInput).value = "";
    }
}

// Conversation Manager - Handles conversation state and logic
class ConversationManager {
    constructor() {
        this.uiManager = new ChatUIManager();
    }

    async loadConversation(rapportId) {
        try {
            console.log("Loading messages for RapportId:", rapportId);
            const messages = await MessageService.fetchConversation(rapportId);
            console.log("Loaded Messages:", messages);
            this.uiManager.renderMessages(messages);
        } catch (error) {
            console.error("Error loading conversation:", error.message);
        }
    }

    async handleConversationClick(rapportId) {
        console.log("Clicked RapportId:", rapportId);
        const conversation = window.conversations?.find(c => c.rapportId === parseInt(rapportId, 10));
        if (!conversation) {
            console.error(`Conversation not found for RapportId: ${rapportId}`);
            return;
        }

        this.setActiveConversation(rapportId, conversation.recipientId);
        await this.loadConversation(rapportId);
    }

    setActiveConversation(rapportId, recipientId) {
        document.querySelector(DOM_SELECTORS.activeConversationRapportId).value = rapportId;
        document.querySelector(DOM_SELECTORS.activeConversationRecipientId).value = recipientId;
    }

    initializeEventListeners() {
        // Initialize conversation card clicks
        const cards = document.querySelectorAll(DOM_SELECTORS.conversationCards);
        if (!cards || cards.length === 0) {
            console.error("No conversation cards found in the DOM.");
            return;
        }

        cards.forEach(card => {
            const rapportId = card.id.split("-")[1];
            if (!rapportId) {
                console.error("Invalid conversation card ID:", card.id);
                return;
            }

            card.addEventListener("click", () => this.handleConversationClick(rapportId));
        });

        // Initialize message form submission
        document.querySelector(DOM_SELECTORS.messageForm).addEventListener("submit",
            (event) => this.handleMessageSubmit(event));
    }

    async handleMessageSubmit(event) {
        event.preventDefault();

        const messageInput = document.querySelector(DOM_SELECTORS.messageInput).value.trim();
        const rapportId = parseInt(document.querySelector(DOM_SELECTORS.activeConversationRapportId).value.trim(), 10);
        const mottakerPersonId = parseInt(document.querySelector(DOM_SELECTORS.activeConversationRecipientId).value.trim(), 10);

        if (!this.validateMessageInput(messageInput, rapportId, mottakerPersonId)) {
            return;
        }

        const formData = this.createMessageFormData(messageInput, rapportId, mottakerPersonId);

        try {
            const result = await MessageService.sendMessage(formData);
            if (result.success) {
                console.log("Message sent successfully:", result.message);
                this.uiManager.clearMessageInput();
                await this.loadConversation(rapportId);
            } else {
                console.error("Failed to send the message:", result.message);
            }
        } catch (error) {
            console.error("Error sending the message:", error.message);
        }
    }

    validateMessageInput(messageInput, rapportId, mottakerPersonId) {
        if (!messageInput) {
            console.error("Message content cannot be empty");
            return false;
        }

        if (isNaN(rapportId) || isNaN(mottakerPersonId) || rapportId <= 0 || mottakerPersonId <= 0) {
            console.error("Please select a conversation before sending a message");
            return false;
        }

        return true;
    }

    createMessageFormData(messageInput, rapportId, mottakerPersonId) {
        const formData = new FormData();
        formData.append('rapportId', rapportId);
        formData.append('mottakerPersonId', mottakerPersonId);
        formData.append('innhold', messageInput);
        return formData;
    }
}

// Initialize the application
document.addEventListener("DOMContentLoaded", () => {
    const conversationManager = new ConversationManager();
    conversationManager.initializeEventListeners();
    conversationManager.uiManager.scrollToBottom();
});