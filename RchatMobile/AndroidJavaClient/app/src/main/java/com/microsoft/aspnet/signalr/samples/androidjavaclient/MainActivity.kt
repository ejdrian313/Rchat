package com.microsoft.aspnet.signalr.samples.androidjavaclient


import android.annotation.SuppressLint
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.provider.Settings
import android.widget.ArrayAdapter
import com.microsoft.aspnet.signalr.HubConnection
import kotlinx.android.synthetic.main.activity_main.*


import java.util.ArrayList

class MainActivity : AppCompatActivity() {

    @SuppressLint("HardwareIds")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        val hubConnection = HubConnection("http://80.211.11.214/chat")
        val idPhone = Settings.Secure.getString(this.contentResolver,
                Settings.Secure.ANDROID_ID)

        val messageList = ArrayList<String>()

        val arrayAdapter = ArrayAdapter(this@MainActivity,
                android.R.layout.simple_list_item_1, messageList)
        lvMessages.adapter = arrayAdapter

        hubConnection.on("broadcastMessage", { name, message-> runOnUiThread(object:Runnable {
            override fun run() {
                arrayAdapter.add("$name: $message")
                arrayAdapter.notifyDataSetChanged()
            }
        }) }, String::class.java, String::class.java)

        bSend.setOnClickListener { view ->
            val message = etMessageText.text.toString()
            etMessageText.setText("")
            try {
                hubConnection.send("Send",idPhone, message)
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }

        try {
            hubConnection.start()
        } catch (e: Exception) {
            e.printStackTrace()
            tvMain.text = e.message
        }
    }
}

