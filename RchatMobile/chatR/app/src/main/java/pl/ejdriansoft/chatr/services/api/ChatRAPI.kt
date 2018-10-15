package pl.ejdriansoft.chatr.services

import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.data.Message
import pl.ejdriansoft.chatr.data.TokenVm
import pl.ejdriansoft.chatr.services.api.ChatRService
import pl.ejdriansoft.chatr.services.api.RequestInterceptor
import retrofit2.Retrofit
import retrofit2.converter.moshi.MoshiConverterFactory
import java.util.concurrent.TimeUnit
import retrofit2.Call

class ChatRAPI {

    private val api: ChatRService
    private val loggingInterceptor = HttpLoggingInterceptor()
    private val requestInterceptor = RequestInterceptor()

    init {
        loggingInterceptor.level = HttpLoggingInterceptor.Level.BODY

        val okHttp = OkHttpClient
                .Builder()
                .retryOnConnectionFailure(true)
                .addInterceptor(requestInterceptor)
                .readTimeout(10, TimeUnit.SECONDS)
                .connectTimeout(10, TimeUnit.SECONDS)
                .addInterceptor(loggingInterceptor)
                .build()

        val retrofit = Retrofit
                .Builder()
                .client(okHttp)
                .baseUrl("http://80.211.11.214/api/")
                .addConverterFactory(MoshiConverterFactory.create())
                .build()

        api = retrofit.create(ChatRService::class.java)
    }

    fun conversations(): Call<List<Conversations>> {
        return api.allMessages()
    }

    fun login(login: String, pass: String) : Call<TokenVm> {
        return api.login(login, pass)
    }
}