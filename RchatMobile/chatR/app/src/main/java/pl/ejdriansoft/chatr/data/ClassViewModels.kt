package pl.ejdriansoft.chatr.data

data class Message(val name: String, val body: String)

data class Conversations(val id: String, val participants: List<String>, val messages: List<Message>)

data class TokenVm(val token: String)