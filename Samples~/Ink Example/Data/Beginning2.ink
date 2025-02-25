INCLUDE Globals.ink
#priority value: 1000
#flags: !game_begun, game_started > 0
#how to handle busy: DELETE
-> beginning2

=== beginning2 ===
{startGame()}{roomieName} shakes {their} head. "No. <i>No</i>. We are not doing this again."

+ [Doing what?]
    "Doing what?" you ask.
    "Don't play dumb."
    ++["Is it the sink?"] 
        "Is it the sink?" <br>{roomieName}'s eyebrows furrow. "No..." {they} says. {They} looks back at the sink.
        {Their} lips press together. "But we're gonna have words about it, later."
    -> END
    ** "The trash?"
    -> END
    ** "You left lint in the drier again?"
    -> END
+ [Yes, we are.]
    You gesture your hands out, palm up. "It appears we are, {roomieName}."
    -> END
+ [<i>We're</i> doing this?]
    ~ score = score + 1
    "<i>We're</i> doing this? I think this is exclusively a <i>you</i> problem."
    -> END