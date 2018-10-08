package pl.ejdriansoft.chatr.services

import pl.ejdriansoft.chatr.data.Message
import retrofit2.Call
import retrofit2.http.GET

interface ChatRService {

    @GET("Dashboard/GetAllMessages")
    fun allMessages() : Call<List<Message>>
}