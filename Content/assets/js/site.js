var isrefing=false;
var tTimaHandler;
$(function(){

    var pg=$("#curPg").val();
   if(pg=="S")$('#S').fire("click");
    else if(pg=="R")$('#R').fire("click");
    else if(pg=="D")$('#D').fire("click");

    $("#ddt").fire("click");
    dispMsg($("#ps").val());

})

    function onItemSelect(val, option, item){
        var tHolda=$(option).attr("data-dDate");
        $("#dispDate").text(tHolda.slice(0,tHolda.lastIndexOf(" ")));
        if(tHolda.search(" ")>-1) $("#dispTime").text(tHolda.slice(tHolda.lastIndexOf(" ")));
        else $("#dispTime").text("");
        $("#dispDescrip").text($(option).attr("data-descrip"));
        $("#dispIntava").text($(option).attr("data-dTime"));
     
        //alert(val);
        if(val== $("#defSto").val()){
            tTimaHandler=setInterval(tblUpdater, 3000);
            doChart(val,$(option).attr("data-dTime"));
            isrefing=true;
        }else {
            $('#tblMain').data('table').loadData('dashboard/fetchtbldata/'+val ,true);            
            clearInterval(tTimaHandler);
            clearInterval(cTimaHandler);
            $("#chtCanvas").html("");
           // alert('there');
        }
        function tblUpdater() {
            $('#tblMain').data('table').loadData('dashboard/fetchtbldata/'+val ,true);
          //  $('#tblMain').data('table').last();
           // alert("done it");
        }
    }

function doXpot(){    //
    var table = $("#tblMain").data('table');
    table.export('CSV', 'all', 'IOTweatherData.csv', {
        csvDelimiter: ",",
        csvNewLine: "\r\n",
        includeHeader: true
    });
}

function PSref(){
    if(isrefing==true){
        clearInterval(tTimaHandler);
        clearInterval(cTimaHandler);
        isrefing=false;
    }
}

function doStore(){
    if(($("#tbxStoTitle").val()=="") ||($("#inpIntval").val()=="")){
        event.preventDefault();
       dispMsg("You must specify the Storage Title and Sampling Interval!");
     }
}

function dispMsg(dMsg=""){
    if(dMsg!=""){
        $("#infoMsg").text(dMsg);
        Metro.getPlugin('#msgBox', 'info-box').open();
    }
}

function doMerge() {
    if($("#tbxMerge").val()==""){
        event.preventDefault();
        dispMsg("You must paste the data from the sdCard before you can proceed.");
    }
}

function doPr(){
    if(($("#tbxOldPs").val()=="")||($("#tbxNewPs").val()=="")){
        event.preventDefault();
        dispMsg("You must provide the Old and New Password");
       }
}