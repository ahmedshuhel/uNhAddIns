Components configuration trough XML

 <component id="someid" type="YourPersistenceConversationalComponent, YourAssembly"
	persistence-conversational="implicit" 
	persistence-conversational-id="yourFixedId" 
	persistence-conversational-idPrefix="yourIdPrefix"
	lifestyle="transient">
  
    <persistence-conversation default-endMode="continue">
      <method name="Validate" exclude="true" />
      <method name="AcceptAll" end-mode="end" />
      <method name="CancelAll" end-mode="abort" />
    </persistence-conversation>
 </component>

component attributes 

	- persistence-conversational : optional if one of the others attributes is present or if <persistence-conversation> section is present.
			"implicit"	: (default) each method is involved in a persistence-conversation if not explicitly excluded.
			"explicit"	: methods involved must be explicitly declared in the <persistence-conversation> section.
			
	- persistence-conversational-id : a fixed Id if all components instances must share the same conversation
	
	- persistence-conversational-idPrefix: useful for logging purpose

  
persistence-conversation section

	- default-endMode: (optional) define the conversation EndMode for all method where not explicit declared
			"continue"	: (default) Resume-Pause of the conversation
			"end"		: end the conversation at the end of method execution
			"abort"		: abort the conversation at the end of method execution
			
	- method : definition of each method involved in a persistence-conversation or methods with a different configuration than default
		name	:	the method name
		exclude :	"true" if you want explicit exclude a method from a persistence-conversation
					"false" otherwise (default)
		end-mode:	"continue" (default) Resume-Pause of the conversation
					"end" end the conversation at the end of method execution
					"abort" abort the conversation at the end of method execution


Configuration combinations

Conversation-per-BusinessTransaction (normal):
 <component id="someid" type="YourPersistenceConversationalComponent, YourAssembly"
	lifestyle="transient">
  
    <persistence-conversation>
      <method name="Validate" exclude="true" />
      <method name="AcceptAll" end-mode="end" />
      <method name="CancelAll" end-mode="abort" />
    </persistence-conversation>
  </component>

Each method of the component is involved in a persistence-conversation excluding the method "Validate".
The UnitOfWork (the nh-session) have the same lifecycle of the conversation.

Conversation-per-BusinessTransaction (WARNING):
 <component id="someid" type="YourPersistenceConversationalComponent, YourAssembly"
	lifestyle="transient">
  
    <persistence-conversation>
      <method name="CancelAll" end-mode="abort" />
    </persistence-conversation>
  </component>
or
 <component id="someid" type="YourPersistenceConversationalComponent, YourAssembly"
	persistence-conversational="implicit" 
	lifestyle="transient">
  
    <persistence-conversation default-endMode="continue">
      <method name="CancelAll" end-mode="abort" />
    </persistence-conversation>
 </component>
or any othe combination excluding an EndMode.End Log a WARNING.

Conversation-per-Call:
 <component id="someid" type="YourPersistenceConversationalComponent, YourAssembly"
	lifestyle="transient">
  
    <persistence-conversation default-endMode="abort">
      <method name="Validate" exclude="true" />
      <method name="Save" end-mode="end" />
      <method name="Delete" end-mode="end" />
    </persistence-conversation>
  </component>

Each method of the component is involved in a persistence-conversation excluding the method "Validate".
The UnitOfWork (the nh-session), because default-endMode="abort", is cleared on each method call excepting
in the method "Save" and "Delete" where the UnitOfWork is committed.