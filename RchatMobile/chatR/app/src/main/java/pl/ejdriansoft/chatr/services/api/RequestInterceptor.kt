package pl.ejdriansoft.chatr.services.api



import okhttp3.Interceptor
import okhttp3.Response
import pl.ejdriansoft.chatr.services.Consts

class RequestInterceptor : Interceptor {
    val headerKey = "No-Authentication"
    override fun intercept(chain: Interceptor.Chain?): Response {
        if (chain == null)
            throw NullPointerException()

        val request = chain.request()
        val requestBuilder = request.newBuilder()
        var token = Consts.token
        if (request.header(headerKey) == null && token != null) {
            requestBuilder.addHeader("Authorization", "Bearer " + token)
        }

        return chain.proceed(requestBuilder.build())
    }
}