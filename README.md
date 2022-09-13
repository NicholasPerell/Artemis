<img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Artemis/Artemis%20Title.png" alt="Artemis Logo. Book aura icon & High shot icon by Lorc." height="152px;" align="center">

>*May your aim be true.*

# ARTEMIS

*Artemis* is an ongoing narrative programming project by [Nicholas Perell](https://nicholasperell.weebly.com/). With the narrative programming done for *[Project Nautilus](https://store.steampowered.com/app/1953870/Project_Nautilus/)*'s narrative system as a basis, the hope is to expand the code to work for anyone's means for delivery (not just emails and subtitled voicelines in *PN*).

As opposed to gems for giving writer the ability to deliver narrative directly, like *Ink* and *Yarnspinner*, Artemus is about the logic that decides what should be deliveried to ensure the narrative beats feel reactive, appropriate, and timely.

## Inspirations

As discussed in [this mircotalk @ The Loaf](https://www.youtube.com/watch?v=iQEwtDx63fw), _Project Nautilus_ took heavy inspiration from _Hades_'s priority queues[^1] and _Firewatch_'s Delilah brain[^2]. However, not every game has nearly as much written content as _Hades_; _Project Nautilus_ used a priority *stack* for there to be recency bias (which *Artemis* allows you to choose between), and *Artemis* will also take inspiration from _Left 4 Dead 2_'s Dynamic Dialog[^3].

## What's Currently Here

This branch is currently working towards **Version 0.2**! It's currently much less user friendly than hoped, but currently in this repository you can find a Unity Project that includes:

- The base code for Artemis's...
  - <ins>Arrows</ins> for each deliverable narrative beat, with ID's, priority values, and what needs to be true or false. 
  - <ins>Archers</ins>, which use the priority values (and when they were added to the archer) to determine which of a group of arrows should be delivered.
  - <ins>Bundles</ins>, which can be prompted to dump into a archer of the designer's choosing.
  - <ins>Feltchers</ins>, which parse .CSV's[^sheets] to generate the databases full of the relevant information needed to direct the...
  - <ins>Bows</ins>, which are the monobehaviors that use the incoming data to deliver the narrative.
  - <ins>Narrative System</ins>, which tracks all the true/false flags the arrows use.
- Example for how the code can be used
  - An example .CSV[^sheets] file.
  - Children scripts of the Feltchers & Bows to deliver debug log messages with a delay before another message can be sent
  - An Editor script that gives the Debug Feltchers to have a button in its inspector to trigger the .CSV[^sheets] parsing.
    - It is reccomended you copy this to use it for your own Feltchers scripts
  - A scene that initializes an example archer then triggers narrative delivery from the archer at a rate that demonstrates the settings each data point can have save for what to do if the bow is busy.

## File-By-File Explanation

Although one of the best ways to get an understanding of *Artemis* would be to check out the examples made, it's worth documenting the target purpose of each of the previously listed items.

### Arrows <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisNarrativeDataPoint%20Icon.png" alt="Scroll unfurled icon by Lorc" height="50px;" align="right">

Stores the most basic information for each possible piece of narrative delivery. This includes:
- <ins>ID:</ins> string used to access the databases found in the feltchers it is connected to.
- <ins>Priority Value:</ins> int value used by the archer
- <ins>Flags to be True:</ins> flags that must be set to true (otherwise the datapoint will be skipped over by the archer)
- <ins>Flags to be False:</ins> flags that must be set to false (otherwise the datapoint will be skipped over by the archer)
- <ins>How to handle busy:</ins> Enum. If the feltchers tries to fire the data point, but the delivery actor is busy, what is done? There are a couple options:
  - CANCEL: Retreat! Return the arrow back to the archer that chose it.
  - QUEUE: Add the datapoint to a queue and wait until the feltcher/bow gets around to it.
  - INTERRUPT: Abruptly stop what the delivery actor is doing to deliver a narrative beat, and have it do this one.
  - DELETE: Don't play it, but don't return it to the archer. Discard the arrow entirely.
  - FRONT_OF_QUEUE: Similar to the queue, but make it cut to the front of the queue.

### Archers <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisNarrativePriorityQueues%20Icon.png" alt="Book pile icon by Delapouite" height="50px;" align="right">

This is what tries to choose which arrow should be shot. Currently uses a priority queue, or PQ. Arrows with a priority of 0 are placed in a "general pool" with a random order. Higher values are given priority above lower values. Instead of being random, there is a checkbox in the inspector about if the PQ should have "recency bais."

When recency bias is *off*, arrows of equal value (greater than 0) will be delivered in the order they were added to the PQ. Hence the term "priority *queue*." 

When recency bias is *on*, the most recently added data point is the one that will delivered first. This was what _Project Nautilus_ utilized for the managerial AI, PASSION. If the player managed to do A and B before they get to the next narrative trigger, and both A and B added commentary the player can receive from PASSION, it made more sense that PASSION commented on the most recent action of the player, to make things feel reactive. Instead of a queue, the PQ becomes a "priority *stack*."

### Bundles <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisNarrativeBundle%20Icon.png" alt="Tied scroll icon by Lorc" height="50px;" align="right">

Can be prompted to dump into a archer of the designer's choosing. These dumps are where recency bias being on or off on your archer are very important. More capabilities and options for bundles are planned for the future, but for now there aren't many.

### Feltchers <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisDeliverySystem%20Icon.png" alt="Dove icon by Lorc" height="50px;" align="right">

_Artemis_'s base feltchers script is an abstract template class, where you will want to define:
 1. The information that needs be stored in a database for the delivery actor to deliver the narrative how you want it.
 2. The `bool SetUpDataFromCells(string[] dataToInterpret, out T valueDetermined)` fuction that validates the string array intake from the .CSV[^sheets] and uses those strings to generate the information that needs to be stored.
 3. The length of the string array. Based on the value of an int named columnsToReadFrom.

To reiterate something said prior: The example folder has an Editor script that gives the Debug Feltchers to have a button in its inspector to trigger the .CSV[^sheets] parsing. It is reccomended you copy this to use it for your own Feltchers scripts.

### Bows <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisDeliveryActor%20Icon.png" alt="High shot icon by Lorc" height="50px;" align="right">

Another whose base script is an abstract template class. The typing on the template class should be the same as the feltchers you want it to work with. This is where things go from decision to full delivery.

To properly set up a script for a delivery actor:
 1. Define `void Send(T data)`. Using this data, how does this gameobject facilitate delivery.
 2. Define `bool IsBusy()`. Is the actor still in the middle of delivery?
 3. Define `void AbruptEnd()`. If the feltchers wants to interrupt with a new data point being sent, how do you wrap up what's going on?
 4. When you're done delivering, be sure to call `ReportEnd();`. This allows the feltchers to see if there were any datapoints with QUEUE or FRONT_OF_QUEUE stored for later.
 5. If you define `OnEnable()`, be sure to call `base.OnEnable();` in the function.

When attaching the delivery actor monobehavior to a game object, make sure the "Feltchers" in the inspector is set to the feltchers you want the actor to be paired with.

### Narrative System <img src="https://github.com/nicholas-hoy-champain/narrative-system-project/blob/75b912e8a91b51cb0f8fffab753287d8f3f26d27/Assets/Gizmos/ArtemisNarrativeSystem%20icon.png" alt="Book aura icon by Lorc" height="50px;" align="right">

Singleton that facilitates the Flags. The flags are their own asset[^whyasset] that store just a bool value and has setter and getter functions for this value. Have **one** (no more, no less) of this asset created.

The narrative system keeps track of if flag assets are being used by any of the arrows generated by the feltcherss. By default, if a flag asset has not a single data point checking it for being true or false, that flag asset will be deleted. However, in the inspector the narrative system has a "Flags to Keep" array. Flag assets in this array will not be deleted by this scrubbing.

## Future Plans

*Artemis* was intially a 6-week project. Given what's here, it has some ways to go with being a robust Unity package, and Perell can see where this can go in the future (and it's an open source project, so having this as an ongoing side-project feels fitting). Some planned additions:

 - The option to make the priority value based off the number of flags it sets off (as opposed to a static value) so *Artemis* can instead emulate bark systems in games like in Left 4 Dead 2[^3].
 - Looking for alternatives to strings for ID's because having symbols would run much better[^3].
 - Giving archers options in regards to refreshing when they're out of arrows.
 - Alternatives to a priority queue for an archer's logic (ex: ranks the arrows by value, but will pull from most valuable arrows at random as if it's another general pool)
 - Bundles tracking if it has already been dumped into (or removed from) a archer.
 - What should be done if the value for a flag hasn't been actively set yet?
 - Save/load capabilities for the whole narrative.
 - More examples and scenes demonstrating how you can use *Artemis*!

## Credits

_Artemis_ is an ongoing narrative programming project by Nicholas Perell.

CSV Parsing Scripts contributed by Brandon "bb" Boras.

Upcoming icons made by Crystal Wong.

### Game-icons.net
Used for the header image & _Artemis_'s Gizmos/Icons
- Archer - Bowman by Lorc
- Flag - Flying Flag by Lorc
- Bundle - Quiver by Delapouite
- Arrow - Branch Arrow by Lorc
- Goddess - Night Sky by Lorc
- Fletcher - Table by Delapouite
- Bow - Bow Arrow by Delapouite

[^sheets]: For what the format of the .CSV's should be like, [here is an example format on Google Sheets](https://docs.google.com/spreadsheets/d/15kqTE368a9-T_mWnH0c4kyO8i_zSbvk4vVvYFh-C_5c/edit?usp=sharing). You're encouraged to make a copy and use it as a basis for yours.
[^1]: [People Make Games's video on _Hades_](https://www.youtube.com/watch?v=bwdYL0KFA_U)
[^2]: [Chris Remo's 2019 GDC talk on _Firewatch_](https://www.youtube.com/watch?v=RVFyRV43Ei8)
[^3]: [Elan Ruskin's 2012 GDC talk on Valve's games](https://www.youtube.com/watch?v=tAbBID3N64A)
[^whyasset]: In [Chris Remo's 2019 GDC talk on _Firewatch_](https://www.youtube.com/watch?v=RVFyRV43Ei8), Remo discussed how their Delilah Brain was also just as much a "best practices" of checking the repository for if a string/flag was made with a slightly different name. Having these generate as assets that can be located in one folder makes it easier to check for that (as opposed to combing through numerous .CSV's row by row).
