INCLUDE Globals.ink
{roomieName} points at the bin and turns {roomie_their} head towards you. "Hey, when are you taking out the trash?"
+ [I took it out last week.]
    "I took it out last week," you say.
    "No, I took it out last week."
    -> END
+ [I'm not?]
    You reply, speaking like it's a question you're asking, "I'm not?"
    {roomieName} rolls {roomie_their} eyes. "C'mon, it's your turn to take out the trash."
    You regret saying it like it was up for discussion. 
    -> END
    +[It's not my turn.]
        "It defintely is."
    -> END
    +["C'mon?"]
        dd
    -> END
    +[Pulease]
        e
    -> END
+ [Next week.]
    "Next week is t
    -> END