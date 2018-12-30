package pl.ejdriansoft.chatr.data

data class Message(val name: String, val body: String)

data class Conversations(val id: String, val nameForeign: String, var lastMessage: String?)

data class TokenVm(val token: String)

data class CreateConversation( val email: String)