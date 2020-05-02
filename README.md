# PIoT

PIoT is a research project/framework for local networking & messaging and IoT development.

As the name suggests the project is aimed at Pi's, but it is cross platform.

The framework represents the network as a graph, containing nodes and links:

* **Node**
  * A device or process on a device.
* **Link**
  * A connection between to nodes.

In a session nodes can transfer messages between each other, if there is no active session messages can be placed in holding.

# Network

## Nodes

Nodes usually reperesent devices, but could be any PIoT process on the network (such as a client or background process).

There is nothing limiting one node to one device, and nodes can contain other inputs (such as a http listener).

### Connections & Listening

Once a `Node` is created, it will start listening for connections.

Nodes can also be instructed to connection to other nodes they can a link to.

## Link

Links represent connections between nodes. 

If a link does not exist the `server` will request informatiom from the `client` to create a new link. 

The link information will be saved in the `Node` settings, so links can be persisted. 

## Sessions

Once a link has been found or created a session is created.

In a session messages can be passed between nodes.

### Initalization & Inherited State

When a new session is initalized, if `inheritState` is true (***default***) both nodes can negotiation a shared state.
This will often involve making sure that are not outstanding actions and delivering holding messages.

If `inheritState` is false, the session will by treated as a "clean slate".
  
# Messaging

Messaging is fundermental to inter-node communitication.

The PIoT messaging system aims to offer an easy and flexible way to handle such tasks as: 

* **Aggregrated Actions**
* **Orchestration**
* **Message Passing** 

## Channels

Channels are how nodes communitate. When the node listener accepts a connection, a channel is created.
The channel id is set to the other nodes id. Data assicoted with the channel can be stored accordingly.

Messages exist on a channel, and can be passed between channels internally.

## Nodes


## Links


## Delivery & Confirmation

Once a message has been recieved and confirmed by a node, it returns a confirmation message.
This simply states the length of the recieved message and a simple hash. This can be verified by the sender if needed.

## Holding

If an active session is not available for a link, messages sent to that link will be placed in holding.

When a session is initalized on the link (and if state is inherited), these messages will be deliveried.

## Handshake

The handshake contains the connecting nodes

## Messages

### Header

### Body
