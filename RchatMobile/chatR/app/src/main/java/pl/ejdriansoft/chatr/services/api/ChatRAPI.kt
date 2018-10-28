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
import retrofit2.http.Query
import javax.net.ssl.HttpsURLConnection
import javax.net.ssl.SSLContext
import javax.net.ssl.TrustManager
import javax.net.ssl.TrustManagerFactory
import android.R.raw
import android.content.Context
import pl.ejdriansoft.chatr.R
import java.security.KeyStore
import java.security.cert.Certificate
import java.security.cert.CertificateFactory


class ChatRAPI(context: Context) {

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
                .baseUrl("https://ejdriansoft.pl/api/")
                .addConverterFactory(MoshiConverterFactory.create())
                .build()

        api = retrofit.create(ChatRService::class.java)
    }

    fun messages(@Query("conversationId") conversationId: String): Call<List<Message>> {
        return api.allMessages(conversationId)
    }

    fun login(@Query("email") login: String, @Query("password") pass: String) : Call<TokenVm> {
        return api.login(login, pass)
    }

    fun conversations() : Call<List<Conversations>> {
        return api.conversations()
    }
}