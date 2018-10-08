package pl.ejdriansoft.chatr.services

import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import pl.ejdriansoft.chatr.data.Message
import retrofit2.Retrofit
import retrofit2.converter.moshi.MoshiConverterFactory
import java.util.concurrent.TimeUnit
import retrofit2.Call

class ChatRAPI {
    //Dashboard/GetAllMessages
    private val api: ChatRService
    private val loggingInterceptor = HttpLoggingInterceptor()

    init {
        loggingInterceptor.level = HttpLoggingInterceptor.Level.BODY

        val okHttp = OkHttpClient
                .Builder()
                .retryOnConnectionFailure(true)
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

    fun allMessages(): Call<List<Message>> {
        return api.allMessages()
    }
}