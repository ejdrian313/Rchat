package pl.ejdriansoft.chatr.services.api

import android.provider.Telephony
import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.data.Message
import pl.ejdriansoft.chatr.data.TokenVm
import retrofit2.Call
import retrofit2.http.GET
import retrofit2.http.Headers

interface ChatRService {

    @GET("Dashboard/Conversations")
    fun allMessages() : Call<List<Conversations>>

    @GET("Authentication/Login")
    @Headers("No-Authentication: true")
    fun login(login: String, pass: String): Call<TokenVm>
}