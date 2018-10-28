package pl.ejdriansoft.chatr.ui

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.ArrayAdapter
import androidx.fragment.app.Fragment
import kotlinx.android.synthetic.main.fragment_conversation.*
import org.jetbrains.anko.*
import org.jetbrains.anko.sdk27.coroutines.onItemClick
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.services.ChatRAPI

class ConversationsFragment : Fragment(), AnkoLogger {

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.fragment_conversation, container, false)
    }

    override fun onStart() {
        super.onStart()
        getMessages((activity as ChatActivity).arrayAdapter)
        lvConversations.adapter = (activity as ChatActivity).arrayAdapter
        lvConversations.onItemClick { adapter, view, position, idd ->
            val item = (activity as ChatActivity).arrayAdapter.getItem(position)
            val args =  Bundle()
            args.putString("id", item)
            val fragment = ChatFragment()
            fragment.arguments = args
            fragmentManager!!.beginTransaction()
                    .replace(R.id.viewConversation, fragment)
                    .commit()
        }
    }

    private fun getMessages(adapter: ArrayAdapter<String>) {
        val call = ChatRAPI(context!!).conversations()
        doAsync {
            val response = call.execute()
            if (response.isSuccessful) {
                uiThread {
                    adapter.clear()
                    response.body()?.forEach {conversation ->
                        adapter.add(conversation.id)
                    }
                    adapter.notifyDataSetChanged()
                    progress?.visibility = View.GONE
                }
            } else {
                uiThread {
                    progress?.visibility = View.GONE
                    (activity as ChatActivity).toast(getString(R.string.error_load_messages)).show()
                }
            }
        }
    }
}
