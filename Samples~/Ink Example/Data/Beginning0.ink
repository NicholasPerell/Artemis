INCLUDE Globals.ink
#priority value: 1000
#flags: !game_begun
#how to handle busy: DELETE
-> beginning0

=== beginning0 ===
{startGame()}{roomieName} points at the bin. "Hey, when are you taking out the trash?
+ [I took it out last week.]
    "I took it out last week," you say.
    "No, I took it out last week."
    -> END
+ [I'm not?]
    You reply, speaking like it's a question you're asking, "I'm not?"
    {roomieName} rolls {roomie_their} eyes. "C'mon, it's your turn to take out the trash."
    You regret saying it like it was up for discussion. 
    ++[It's not my turn.]
        "It's not my turn."
        "It defintely is."
    -> END
    ++["C'mon?"]
        "'C'mon?'" you say. "Next you gonna ask Tony to speedwalk over 'ta getcha uh slice?'"
        ~ score = score - 1
        {roomieName} winces. "That wasn't even remotely funny. And yeah, take out the trash."
        "You're so sure it's my turn?"<br>"Yeah."
        ~ childishMoments = childishMoments + 1
        You send a limp wrist {roomie_their} way. "'Fuggedaboutit.'" — you continue before {roomieName} can let out an audible groan — "I took it out last week, {roomieName}."
        "No, you didn't." <br>"Yes, I did. I remember; it was last week."
        "Oh c'mon," {roomie_they} says. You jut your head forward and {roomie_they} rolls {roomie_their} eyes away from you.
        ***"I took it out the night I heated the Hawaiian rolls."
        ~ hawaiianRolls = true
        -> END
        ***"I took it out the night I found your fried stuff in the fridge."
        ~ friedFoodLastWeek = true
        -> END
    ++[Pulease]
        e
    -> END
+ [Next week.]
    "Next week is t
    ~ score = score + 100

    -> END
    
=== knotName ===
This is the content of the knot.
-> END