package pl.ejdriansoft.chatr.ui

import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.crashlytics.android.Crashlytics
import io.fabric.sdk.android.Fabric
import kotlinx.android.synthetic.main.activity_login.*
import org.jetbrains.anko.intentFor
import org.jetbrains.anko.singleTop
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.api.Prefs

class LoginActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Fabric.with(this, Crashlytics())
        setContentView(R.layout.activity_login)
        val prefs = Prefs(this)

        if (prefs.nickname?.isNotEmpty()!!) {
            startActivity(intentFor<ChatActivity>().singleTop())
            finish()
        }

        btnChat.setOnClickListener {
            if (etNickname.text.isNullOrBlank()) {
                etNickname.error = "Choose a pokemon, Harry Potter"
                return@setOnClickListener
            } else {
                if (etNickname.text.toString() == "buk") {
                    etNickname.error = "Ja jestem bogiem, tylko wyobraź to sobie sobie"
                    return@setOnClickListener
                }
                etNickname.error = null
                prefs.nickname = etNickname.text.toString()
                startActivity(intentFor<ChatActivity>().singleTop())
                finish()
            }
        }
    }
}
