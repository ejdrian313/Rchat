package pl.ejdriansoft.chatr.ui.adapters

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.AdapterView
import androidx.core.content.res.ResourcesCompat
import androidx.fragment.app.FragmentManager
import androidx.recyclerview.widget.RecyclerView
import kotlinx.android.synthetic.main.item_conversation.view.*
import org.jetbrains.anko.sdk27.coroutines.onClick
import pl.ejdriansoft.chatr.R
import pl.ejdriansoft.chatr.data.Conversations
import pl.ejdriansoft.chatr.ui.ChatFragment

class ConversationAdapter(
        private val items : ArrayList<Conversations>,
        private val context: Context,
        private val fm: FragmentManager
) : RecyclerView.Adapter<ViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        return ViewHolder(LayoutInflater.from(context).inflate(R.layout.item_conversation, parent, false))
    }

    override fun getItemCount() = items.count()

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        holder.tvConversationName?.text = items[position].nameForeign
        holder.tvConversationId?.text = items[position].id
        holder.tvLastMessage?.text = items[position].lastMessage
        holder.clConversations.onClick {
            val args =  Bundle()
            args.putString("id", holder.tvConversationId!!.text.toString())
            val fragment = ChatFragment()
            fragment.arguments = args
            fm.beginTransaction()
                    .add(R.id.viewConversation, fragment)
                    .addToBackStack("conversation")
                    .commit()
        }
    }

}

class ViewHolder (view: View) : RecyclerView.ViewHolder(view) {
    val tvConversationName = view.tvConversationName
    val tvConversationId = view.tvConversationId
    val tvLastMessage = view.tvLastMessage
    val clConversations = view.clConversation
}