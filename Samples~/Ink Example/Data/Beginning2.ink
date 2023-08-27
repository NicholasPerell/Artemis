INCLUDE Globals.ink
{roomieName} shakes {roomie_their} head. "No. <i>No</i>. We are not doing this again."

+ [Doing what?]
    "Doing what?" you ask.
    "Don't play dumb."
    -> END
+ [Yes, we are.]
    You gesture your hands out, palm up. "It appears we are, {roomieName}."
    -> END
+ [<i>We're</i> doing this?]
    "<i>We're</i> doing this? I think this is exclusively a <i>you</i> problem."
    ~ score = score + 1
    -> END