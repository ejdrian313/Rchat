package pl.ejdriansoft.chatr


import android.os.Bundle
import android.os.Message
import android.provider.Settings
import android.provider.Settings.Secure.getString
import android.widget.ArrayAdapter
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.crashlytics.android.Crashlytics
import com.google.gson.Gson
import com.smartarmenia.dotnetcoresignalrclientjava.*
import com.squareup.moshi.Moshi
import io.fabric.sdk.android.Fabric
import kotlinx.android.synthetic.main.activity_main.*
import org.jetbrains.anko.*
import pl.ejdriansoft.chatr.services.ChatRAPI
import java.util.ArrayList


class MainActivity : AppCompatActivity(), HubConnectionListener, HubEventListener, AnkoLogger {

    lateinit var arrayAdapter: ArrayAdapter<String>
    private lateinit var hubConnection: WebSocketHubConnectionP2

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Fabric.with(this, Crashlytics())
        setContentView(R.layout.activity_main)

        val hubConnection = WebSocketHubConnectionP2("http://80.211.11.214/chat", "")
        val idPhone = getString(this.contentResolver,
                Settings.Secure.ANDROID_ID)

        val messageList = ArrayList<String>()
        arrayAdapter = ArrayAdapter(this@MainActivity,
                android.R.layout.simple_list_item_1, messageList)
        lvMessages.adapter = arrayAdapter
        getMessages(arrayAdapter)

//        connectHub(hubConnection, arrayAdapter)

        bSend.setOnClickListener { _ ->
            val message = etMessageText.text.toString()
            etMessageText.setText("")
            try {
                hubConnection.invoke("Send", idPhone, message)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
        startHubConnection(hubConnection)

        hubConnection.subscribeToEvent("broadcastMessage", this)

    }

    private fun getMessages(adapter: ArrayAdapter<String>) {
        val call = ChatRAPI().allMessages()
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                runOnUiThread {
                    response.body()?.forEach {
                        adapter.add("${it.name}: ${it.body}")
                    }
                    adapter.notifyDataSetChanged()
                    lvMessages.smoothScrollToPosition(adapter.count)
                }
            } else {
                runOnUiThread {
                    toast("Error loading messages").show()
                }
            }
        }
    }

    private fun startHubConnection(hub: HubConnection) {
        try {
            hub.connect()
        } catch (e: Exception) {
            e.printStackTrace()
            tvMain.text = e.message
        }
    }

    override fun onEventMessage(message: HubMessage) {
        runOnUiThread {
            runOnUiThread {
                info { "${message.target}\n${Gson().toJson(message.arguments)}" }
                //info {Gson().toJson(message.arguments)}
                //Moshi().adapter(Message::class.java).fromJson(Gson().toJson(message.arguments))
                arrayAdapter.add("${message.arguments[0].toString()}: ${message.arguments[1].toString()}")
                arrayAdapter.notifyDataSetChanged()
            }
        }
    }

    override fun onConnected() {
        runOnUiThread { Toast.makeText(this@MainActivity, "Connected", Toast.LENGTH_SHORT).show() }
    }

    override fun onDisconnected() {
        runOnUiThread { Toast.makeText(this@MainActivity, "Disconnected", Toast.LENGTH_SHORT).show() }
    }

    override fun onMessage(message: HubMessage) {
        runOnUiThread {
            Toast.makeText(this@MainActivity, "${message.target}\n${Gson().toJson(message.arguments)}", Toast.LENGTH_SHORT).show()
        }
    }

    override fun onError(exception: Exception) {
        runOnUiThread { Toast.makeText(this@MainActivity, exception.message, Toast.LENGTH_SHORT).show() }
    }

    override fun onDestroy() {
        super.onDestroy()
        hubConnection.removeListener(this)
        hubConnection.unSubscribeFromEvent("Send", this)
        hubConnection.disconnect()
    }
}

