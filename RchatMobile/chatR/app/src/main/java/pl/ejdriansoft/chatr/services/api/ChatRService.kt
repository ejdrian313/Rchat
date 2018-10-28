package pl.ejdriansoft.chatr.services.api

import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.data.Message
import pl.ejdriansoft.chatr.data.TokenVm
import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Headers
import retrofit2.http.Query

interface ChatRService {

    @GET("Dashboard/GetAllMessages")
    fun allMessages(@Query("conversationId") conversationId: String) : Call<List<Message>>

    @GET("Dashboard/Conversations")
    fun conversations() : Call<List<Conversations>>

    @GET("Authentication/Login")
    @Headers("No-Authentication: true")
    fun login(@Query("email") login: String, @Query("password") pass: String): Call<TokenVm>
}