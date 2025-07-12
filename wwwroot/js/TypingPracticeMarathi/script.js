let isCapsLockOn = 0;
let paragraphLength = 0;
let currentChar = "";
let currentCharkeyCode = 0;
let currentCharPosition = 1;
let isTimerStart = 0;
let keyPressAudio = 0;
let errorAudio = 0;
$(document).ready(function () {
    keyPressAudio = $("#keypress")[0];
    errorAudio = $("#error")[0];
    // Ensure audio does not loop
    keyPressAudio.loop = false;
    errorAudio.loop = false;
    $(document).keydown(function (event) {
        if (isTimerStart == 0) {
            startTimer(1)
            isTimerStart = 1;
        }
        console.log("Key Code: " + event.keyCode);
        console.log("Key Pressed: " + event.key);

        console.log($('.key-label').text());

        $(`.key-${event.keyCode}`).animate();
        if (event.keyCode == 20) {
            if (isCapsLockOn == 0) {
                $(".key").css("text-transform", "uppercase");
                isCapsLockOn = 1;
            } else {
                $(".key").css("text-transform", "lowercase");
                isCapsLockOn = 0;
            }
        }
        if (event.keyCode >= 65 && event.keyCode <= 90) {
            $(`.key-${event.keyCode + 32}`).css("background-color", "green"); // Change to red immediately
            setTimeout(() => {
                $(`.key-${event.keyCode + 32}`).css("background", "none"); // Change back after 1 second
            }, 100);
            StartTyping(event.key, event.keyCode);
        } else if (
            event.keyCode == 32 ||
            (event.keyCode >= 48 && event.keyCode <= 57)
        ) {
            $(`.key-${event.keyCode}`).css("background-color", "green"); // Change to red immediately
            setTimeout(() => {
                $(`.key-${event.keyCode}`).css("background", "none"); // Change back after 1 second
            }, 50);
            StartTyping(event.key, event.keyCode);
        } else if (event.key != "Shift") {
            //alert(event.key+'\n'+event.keyCode)
            $(`.key-${event.keyCode}`).css("background-color", "green"); // Change to red immediately
            setTimeout(() => {
                $(`.key-${event.keyCode}`).css("background", "none"); // Change back after 1 second
            }, 50);
            StartTyping(event.key, event.keyCode);
        }
    });
    //
    CheckFirst();
});
function charToKeycode(char) {
    return char.charCodeAt(0);
}
function StartTyping(key, code) {
    Check(charToKeycode(key));
}
function Check(code) {
    paragraphLength = $(".ch").length;
    console.log(code);
    if (paragraphLength >= currentCharPosition) {
        currentChar = $(`.c-${currentCharPosition}`)
            .html()
            .replace(/&nbsp;/g, " ").replace(/&gt;/g, ">").replace(/&lt;/g, "<");;
        currentCharkeyCode = charToKeycode(currentChar);
        console.log(currentCharkeyCode + " === " + code);
        if (currentCharkeyCode === code) {
            keyPressAudio.currentTime = 0;
            keyPressAudio.play();
            DeactiveKey();
            currentCharPosition++;
            ActiveKey();
        } else {
            errorAudio.currentTime = 0;
            errorAudio.play();
            $(`.c-${currentCharPosition}`).addClass("text-error");
        }
    }
}
function CheckFirst() {
    currentChar = $(`.c-${currentCharPosition}`)
        .html()
        .replace(/&nbsp;/g, " ").replace(/&gt;/g, ">").replace(/&lt;/g, "<");
    currentCharkeyCode = charToKeycode(currentChar);
    $(`.c-${currentCharPosition}`).addClass("text-active");
    currentChar = $(`.c-${currentCharPosition}`)
        .html()
        .replace(/&nbsp;/g, " ").replace(/&gt;/g, ">").replace(/&lt;/g, "<");
    $(`.key-${charToKeycode(currentChar)}`).addClass('key-active');
    HandGesture(charToKeycode(currentChar))
}
async function DeactiveKey() {
    $(`.c-${currentCharPosition}`).removeClass("text-active");
    $(`.c-${currentCharPosition}`).addClass("text-good");
}
async function ActiveKey() {
    $(`.key`).removeClass('key-active')
    $(`.c-${currentCharPosition}`).addClass("text-active");
    currentChar = $(`.c-${currentCharPosition}`)
        .html()
        .replace(/&nbsp;/g, " ").replace(/&gt;/g, ">").replace(/&lt;/g, "<");
    $(`.key-${charToKeycode(currentChar)}`).addClass('key-active');
    HandGesture(charToKeycode(currentChar))
}
function HandGesture(currentCharkeyCode) {
    if (currentCharkeyCode == 97) {//a
        //alert()
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-5.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 98) {//b
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-bottom-row-1.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 99) {//c
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-bottom-row-3.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 100) {//d
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-3.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 101) {//e
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-top-row-3.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 102) {//f
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-2.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 103) {//g
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-1.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 104) {//h
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-home-row-1.webp)');
    }
    else if (currentCharkeyCode == 105) {//i
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-3.webp)');
    }
    else if (currentCharkeyCode == 106) {//j
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-bottom-row-2.webp)');
    }
    else if (currentCharkeyCode == 107) {//k
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-3.webp)');
        $('#rightHand').css('margin-top', '80px');
    }
    else if (currentCharkeyCode == 108) {//l
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-4.webp)');
        $('#rightHand').css('margin-top', '80px');
    }
    else if (currentCharkeyCode == 109) {//m
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-bottom-row-2.webp)');
        $('#rightHand').css('margin-top', '60px');
    }
    else if (currentCharkeyCode == 110) {//n
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-bottom-row-1.webp)');
    }
    else if (currentCharkeyCode == 111) {//o
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-4.webp)');
        $('#rightHand').css('margin-top', '40px');
    }
    else if (currentCharkeyCode == 112) {//p
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-5.webp)');
        $('#rightHand').css('margin-top', '40px');
    }
    else if (currentCharkeyCode == 113) {//q
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-top-row-5.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');

    }
    else if (currentCharkeyCode == 114) {//r
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-top-row-1.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 115) {//s
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-4.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 116) {//t
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-top-row-2.webp)');
        $('#leftHand').css('margin-left', '-5px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 117) {//u
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-2.png)');
        $('#rightHand').css('margin-left', '-45px');
    }
    else if (currentCharkeyCode == 118) {//v
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-bottom-row-1.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#leftHand').css('margin-left', '-45px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
        $('#rightHand').css('margin-left', '0');
    }
    else if (currentCharkeyCode == 119) {//w
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-home-row-4.webp)');
        $('#leftHand').css('margin-top', '25px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 120) {//x
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-bottom-row-3.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else if (currentCharkeyCode == 121) {//y
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-top-row-2.png)');
        $('#rightHand').css('margin-left', '-55px');
    }
    else if (currentCharkeyCode == 122) {//z
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-bottom-row-3.webp)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
        $('#rightHand').css('margin-left', '0');
    }
    else if (currentCharkeyCode == 32) {//space
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#leftHand').css('margin-top', '50px');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }
    else {
        $('#leftHand').css('background-image', 'url(/assets/images/handgesture/left-resting-hand.png)');
        $('#rightHand').css('background-image', 'url(/assets/images/handgesture/right-resting-hand.webp)');
    }

}
