//Roommate Name & Pronouns
VAR roomieName = "Matt"
VAR roomie_they = "he"
VAR roomie_them = "him"
VAR roomie_their = "his"
VAR roomie_They = "He"
VAR roomie_Them = "Him"
VAR roomie_Their = "His"

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
    ~ roomieName = "{~Matt|Ryan|John|Louie|Chris|Davey}"
