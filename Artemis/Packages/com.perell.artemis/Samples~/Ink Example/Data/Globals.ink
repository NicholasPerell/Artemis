//Roommate Name & Pronouns
VAR roomieName = "Roomie"
VAR they = "they"
VAR them = "them"
VAR their = "their"
VAR They = "They"
VAR Them = "Them"
VAR Their = "Their"

//Roommate Score (10 is a win)
VAR score = 0

//Convo Flags
VAR game_begun = false
VAR game_started = 0
VAR childishMoments = 0
VAR playerLanguage = false
VAR roomieLanguage = false
VAR nuhUh = false
VAR hawaiianRolls = false
VAR friedFoodLastWeek = false

=== function startGame() ===
    ~ game_begun = true
    ~ game_started = game_started + 1
    
    //Determine Roommate Name
    ~ roomieName = "{~Matt|Ryan|John|Louie|Chris|Davey|Jane|Katy|Jess|Meg|Candy|Olivia|Kit|Avery|Ash|Chey|Jay|Sage}"
    
    //Determine Roommate Pronouns
    {
    - roomieName == "Matt" || roomieName == "Ryan" || roomieName == "John" || roomieName == "Louie" || roomieName == "Chris" || roomieName == "Davey":
            ~ they = "he"
            ~  them = "him"
            ~  their = "his"
            ~  They = "He"
            ~  Them = "Him"
            ~  Their = "His"
    - roomieName == "Jane" || roomieName == "Katy" || roomieName == "Jess" || roomieName == "Meg" || roomieName == "Candy" || roomieName == "Olivia":
            ~ they = "she"
            ~  them = "her"
            ~  their = "her" 
            ~  They = "She"
            ~  Them = "Her"
            ~  Their = "Hers"
        - else:
            ~ they = "they"
            ~  them = "them"
            ~  their = "their"
            ~  They = "They"
            ~  Them = "Them"
            ~  Their = "Their"
    }

=== function s(verb) ===
    {
    - they == "they":
        ~ return verb
    - else:
        ~ return verb + "s"
    }
