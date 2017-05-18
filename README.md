# Welcome to ChatLoco - *The best location based Chat Room Service around*

ChatLoco is a web application that enables users to freely, and anonymously talk to other users through various chatrooms 
completely based off their geo location (thanks to Google's location API).


## Okay...but what REALLY is ChatLoco?

ChatLoco began as 5 undergraduates capstone project in order to graduate. After developing the very beginning, we kinda 
took it to infinity and beyond to make a very robust and polished web application.
**ChatLoco** utilizes the users location to determine which chatrooms are available in their area, giving them 
a slide bar which the user can use to grow or shrink their "search radius". This enables multiple users to be 
geographically in the same location, and chat with eachother in both public, and private chatrooms in an IRC fashion.
Users sign up for the ChatLoco services using a username that is unique and specific to that user, but when entering
any chatroom, they are allowed to pick a handle, thus giving them the option for anonymity if they so choose.


## What is the difference between public and private?

There can only ever be 1 public chatroom in any given spot since the room itself is actually constructed on top of a 
Google "marker" which they deemed a "location". Users in the public chatroom use it similar to IRC, in that everyone can
see everyones messages and there is most always (if the chatroom is populated) something going on. There is a whisper
functionality which allows two users within the public catroom to talk to ONLY one another - but ill elaborate later.
There can be any number of private chatrooms specific to one location. This is because the provate chatrooms are user 
created. Users can set passwords, participant capacity, and a blacklist if they wish on the room.


## What can you do in a chatroom?

This is the fun part. So our text field allows for all standard ascii characters. We also supply a dropdown for text
color selection, in addition to a **bold** and *italics* option.
There are two commands that allow the user to interact a little more...awesomely...within the chatroom.

Beginning a message with "/whisper" followed by the username you with to whisper to, then followed by the message allows
for only the sender and recipient to view the message EVEN when in the public chatroom.
```
/whisper fontaine Hey buddy, so only you and I can see this?!
```
Beginnind a message with "/img" followed by any image or gif url will embed the image or gif in the chatroom. If it's a 
gif, it will play within the chatroom like any high quality chatroom application would!
```
/img http://thisUrl-would+play-intheChatroom.gif
```

## Awesome! Where can I go to access the site?

This is the reason for such a verbose README...[ChatLoco](https://www.chatlo.co)
So the short answer is the site is dark at the moment. This is SOLELY because of funding. 
We wrote the application in C# using ASP.NET MVC5, with Entity Framework for our database so when it came time to host it
we used Microsoft Azure. This was fantastic as their hosting service is SO robust, however we paid a pretty penny to use 
it. Google also required our site use https and have an official SSL certificate from a trusted CA - which we received 
from [Let's Encrypt](https://letsencrypt.org). Lastly we bought the domain *chatlo.co* and thus need to continue payment 
for continued use.
The big kicker was Azure, and after the class ended we (as a group) decided to cancel the monthly funding for the 
application. We did however decide to keep the domain so if we ever wanted to go live again, we could.
So that is why ChatLoco is dark :C 


## That sucks...but I don't have an infinite wallet either so I understand

But there is hope! Knowing the services would be shut off, we compiled a bunch of sccreenshots from within the app of all
the core features and posted it in a wiki on this github so you can see it in its glory days, even if the site is all but
dark.



### Meet the Devs - and their githubs!
* [Angelo Poulos](https://github.com/anpoulos)
* [Cooper Patton](https://github.com/zerocoolx)
* [Cole Prinsen](https://github.com/cprinsen)
* [Shane Sedgwick](https://github.com/sedgsha)
* [Jake Medal](https://github.com/jakemedal)


## Dev notes
Thats all she wrote boys. gg
*05/18/17 ChatLoco goes dark*
