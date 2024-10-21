# ProgressionLock
A simple v5 TShock plugin that deletes items based on the world progression. Original code by [Ozz](https://github.com/Ozz5581/AntiPlat)https://github.com/Ozz5581/AntiPlat
This plugin is suited for my server, so feel free to edit it to remove or add items.
I am very new to coding so I simply stumble my way through stuff to get things to work how I want them to.

This plugin includes a new command called "/resetworldprogress" under the permission node "pbac.resetboss"

Do note that it could be laggy due to how it checks each slot of each player. So pardon my spaghetti code.

Something I plan on doing is making it so all the items are located neatly in a config file since currently it's just directly in a large if statements and a command which allows you add the items directly with command and make the notification be sent to either only on console or .
This isn't really an anti-cheat, just something to prevent unobtainable items from being distributed or used if X boss or event is not done.

Permissions:

- pl.bypass (Makes group immune to progressionlock item deletion.)
