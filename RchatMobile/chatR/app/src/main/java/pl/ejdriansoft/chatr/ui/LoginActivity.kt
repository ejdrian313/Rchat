package pl.ejdriansoft.chatr.ui

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.crashlytics.android.Crashlytics
import io.fabric.sdk.android.Fabric
import kotlinx.android.synthetic.main.activity_login.*
import org.jetbrains.anko.doAsync
import org.jetbrains.anko.intentFor
import org.jetbrains.anko.singleTop
import org.jetbrains.anko.toast
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI
import pl.ejdriansoft.chatr.services.Consts
import pl.ejdriansoft.chatr.services.Prefs

class LoginActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Fabric.with(this, Crashlytics())
        setContentView(R.layout.activity_login)
        val prefs = Prefs(this)
        if (prefs.token?.isNotEmpty()!!) {
            Consts.token = prefs.token
            startActivity(intentFor<ChatActivity>().singleTop())
            finish()
        }

        val api = ChatRAPI()
        btnLogin.setOnClickListener {
            if (etNickname.text.isNullOrBlank()) {
                etNickname.error = "Enter login"
                return@setOnClickListener
            }
            if (etPassword.text.isNullOrBlank()) {
                etPassword.error = "Enter password"
                return@setOnClickListener
            }
            if (etNickname.text.toString() == "buk") {
                etNickname.error = "Ja jestem bogiem, tylko wyobraź to sobie sobie"
                return@setOnClickListener
            }

            etNickname.error = null

            val call = api.login(etNickname.text.toString(), etPassword.text.toString())

            doAsync {
                val response = call.execute()

                if (response.isSuccessful) {
                    val token = response.body()?.token
                    Consts.token = token
                    prefs.token = token
                    startActivity(intentFor<ChatActivity>().singleTop())
                    finish()
                } else {
                    runOnUiThread {
                        toast("Login failed").show()
                    }
                }
            }
        }
    }
}
