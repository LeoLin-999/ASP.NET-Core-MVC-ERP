// 時間表
function currentClock(_Clock)
{
    var current = new Date();
    var currentYear = current.getFullYear();
    var currentMonth = current.getMonth() + 1;
    var currentDate = current.getDate();
    var currentHour = current.getHours();
    var currentMinute = current.getMinutes();
    var currentSecond = current.getSeconds();
    var show = currentYear + "年" + (currentMonth < 10 ? ("0" + currentMonth) : currentMonth) + "月" + (currentDate < 10 ? ("0" + currentDate) : currentDate) + "日 " + (currentHour < 10 ? ("0" + currentHour) : currentHour) + "時" + (currentMinute < 10 ? ("0" + currentMinute) : currentMinute) + "分" + (currentSecond < 10 ? ("0" + currentSecond) : currentSecond) + "秒";
    document.getElementById(_Clock).innerHTML = show;

    // 每隔一秒跑一次
    setTimeout("currentClock('" + _Clock + "')", 1000);
}