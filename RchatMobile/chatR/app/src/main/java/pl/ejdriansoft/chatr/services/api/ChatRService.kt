package pl.ejdriansoft.chatr.services.api

import pl.ejdriansoft.chatr.data.Message
import pl.ejdriansoft.chatr.data.TokenVm
import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Headers
import retrofit2.http.Query

interface ChatRService {

    @GET("Dashboard/GetAllMessages")
    fun allMessages() : Call<List<Message>>

    @GET("Authentication/Login")
    @Headers("No-Authentication: true")
    fun login(@Query("email") login: String, @Query("password") pass: String): Call<TokenVm>
}