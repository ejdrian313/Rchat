package pl.ejdriansoft.chatr.ui


import android.annotation.SuppressLint
import android.os.Bundle
import android.view.View
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.google.gson.Gson
import com.smartarmenia.dotnetcoresignalrclientjava.*
import kotlinx.android.synthetic.main.activity_chat.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI
import pl.ejdriansoft.chatr.services.Consts
import pl.ejdriansoft.chatr.services.Prefs
import java.util.ArrayList


class ChatActivity : AppCompatActivity(), HubConnectionListener, HubEventListener, AnkoLogger {

    lateinit var arrayAdapter: ArrayAdapter<String>
    lateinit var hubConnection: WebSocketHubConnectionP2

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)

        hubConnection = WebSocketHubConnectionP2("https://ejdriansoft.pl/chat", "Bearer ${Consts.token}")

        val messageList = ArrayList<String>()
        arrayAdapter = ArrayAdapter(this@ChatActivity,
                R.layout.item_chat, messageList)
        supportFragmentManager.beginTransaction()
                .replace(R.id.viewConversation, ConversationsFragment())
                .commit()

        startHubConnection()
        subscribeHub()
    }

    override fun onBackPressed() {
        if (supportFragmentManager.backStackEntryCount > 1) {
            supportFragmentManager.popBackStack()
        } else {
            super.onBackPressed()
        }
    }

    @SuppressLint("SetTextI18n")
    private fun subscribeHub() {
        hubConnection.subscribeToEvent("broadcastMessage", this)
        hubConnection.subscribeToEvent("currentConnections") { message ->
            runOnUiThread {
                tvMain.text = "Current connections: ${message.arguments[0].asString}"
            }
        }
    }

    override fun onResume() {
        super.onResume()
        startHubConnection()
    }

    private fun startHubConnection() {
        try {
            hubConnection.connect()
        } catch (e: Exception) {
            e.printStackTrace()
        }
    }

    override fun onEventMessage(message: HubMessage) {
        val name = message.arguments[0].asString
        val body = message.arguments[1].asString
        val conversationId = message.arguments[2].asString
        runOnUiThread {
            arrayAdapter.add("$name: $body")
            arrayAdapter.notifyDataSetChanged()
        }
    }

    override fun onConnected() {
        runOnUiThread { Toast.makeText(this@ChatActivity, "Connected", Toast.LENGTH_SHORT).show() }
    }

    override fun onDisconnected() {
        runOnUiThread { Toast.makeText(this@ChatActivity, "Disconnected", Toast.LENGTH_SHORT).show() }
    }

    override fun onMessage(message: HubMessage) {
        runOnUiThread {
            Toast.makeText(this@ChatActivity, "${message.target}\n${Gson().toJson(message.arguments)}", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onError(exception: Exception) {
        runOnUiThread { Toast.makeText(this@ChatActivity, exception.message, Toast.LENGTH_SHORT).show() }
    }
}

