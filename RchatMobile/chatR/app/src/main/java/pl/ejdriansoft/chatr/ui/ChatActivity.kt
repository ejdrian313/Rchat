package pl.ejdriansoft.chatr.ui


import android.annotation.SuppressLint
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.google.gson.Gson
import com.smartarmenia.dotnetcoresignalrclientjava.*
import com.squareup.moshi.Moshi
import kotlinx.android.synthetic.main.activity_chat.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.data.Message
import pl.ejdriansoft.chatr.services.Consts
import java.util.ArrayList


class ChatActivity : AppCompatActivity(), HubConnectionListener, HubEventListener, AnkoLogger {

    val conversationsHashMap = LinkedHashMap<Conversations, List<Message>?>()
    lateinit var messageAdapter: ArrayAdapter<String>
    lateinit var hubConnection: WebSocketHubConnectionP2
    val moshi = Moshi.Builder().build()
    var messageJson = moshi.adapter(Message::class.java)

    val conversations = ArrayList<Conversations>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)

        hubConnection = WebSocketHubConnectionP2("https://ejdriansoft.pl/chat", "Bearer ${Consts.token}")

        val conversations = ArrayList<String>()

        messageAdapter = ArrayAdapter(this@ChatActivity,
                R.layout.item_chat, conversations)

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

    override fun onEventMessage(hubMessage: HubMessage) {
        val message = messageJson.fromJson(hubMessage.arguments[0].toString())
        runOnUiThread {
            messageAdapter.add("${message.name}: ${message.body}")
            messageAdapter.notifyDataSetChanged()
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

