
var cTimaHandler;

function doChart(dID="",dIntval){
        Highcharts.chart('chtCanvas', {
            chart: {
                type: 'spline',
                animation: true,// Highcharts.svg, // don't animate in old IE
                marginRight: 10,
                zoomType: 'y,x',
                events: {
                    load: function () {
                        // set up the updating of the chart each second
                        var pressure = this.series[0];
                        var temp = this.series[1];
                        var humidity = this.series[2];
                        var light= this.series[3];
                        var rDS= this.series[4];
                        var rainFreq= this.series[5];
                        var rainGauge= this.series[6];
                        var retVal;
                        var itr=0;
                        var tStat=0;
                        var isF=true;
                        cTimaHandler= setInterval(upDateChart,3000);

                        function upDateChart () {
                            $.ajax({
                                url: "dashboard.php",
                                method: "GET",
                                timeout:2000,
                                data: {toStat: tStat,datId:dID }
                            }).then(
                                function(response){  //alert(response);
                                    if(response!='non'){
                                        retVal=JSON.parse(response);
                                        for ( itr=0;itr<retVal.length;itr++){
                                            pressure.addPoint([Number(retVal[itr].p)], true,false,true);
                                            temp.addPoint([ Number(retVal[itr].t)], true,false,true);
                                            humidity.addPoint([Number(retVal[itr].h)], true,false,true);
                                            light.addPoint([Number(retVal[itr].l)], true,false,true);
                                            rDS.addPoint([Number(retVal[itr].d)], true,false,true);
                                            rainFreq.addPoint([Number(retVal[itr].f)], true,false,true);
                                            rainGauge.addPoint([Number(retVal[itr].g)], true,false,true);
                                            //   pressure.addPoint([ retVal[itr].pressCol], true,false,true);
                                        }
                                        if(isF){tStat=itr++; isF=false;}
                                        else tStat=tStat+1;  // alert(tStat);
                                    }
                                },
                                function(xhr){
                                    //alert(xhr.status+ '\n'+ xhr.statusText);
                                }
                            )
                        }
                    }
                },
                scrollablePlotArea: {
                    minWidth: 700
                }
            },
            title: {
                text: 'Live data plot'
            },
            subtitle: {
                text: 'Source: values from the remote sensors'
            },
            xAxis: {
                type: 'number',
                tickPixelInterval: 150,
                title: {
                    text: 'Time(Mins)'
                }
            },

            yAxis: {
                title: {
                    text: 'Meansured Value'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },

            tooltip: {
                headerFormat: '<b>{series.name}</b><br/>',
                pointFormat: '<span>X:{point.x:.2f}<br/>Y:{point.y:.2f}</span>'
            },

            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },
            responsive: {
                rules: [{
                    condition: {
                        maxWidth: 500
                    },
                    chartOptions: {
                        legend: {
                            layout: 'horizontal',
                            align: 'center',
                            verticalAlign: 'bottom'
                        }
                    }
                }]
            },

            exporting: {
                enabled: false
            },
            plotOptions: {
                series: {
                    pointStart: 0,
                    pointInterval:Number(dIntval),  //remeember to supply the varible
                }
            },
            series: [{
                name: 'Pressure(KPa)',
                data: (function () {
                    var data = [];
                    return data;
                }())
            },
                {
                    name: 'Temperature(°C)',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                },
                {
                    name: 'Humidity(%)',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                },
                {
                    name: 'Light(lux)',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                },
                {
                    name: 'RDS(mm)',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                },
                {
                    name: 'Rain Rate',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                },
                {
                    name: 'Rain Gauge(mil)',
                    data: (function () {
                        var data = [];
                        return data;
                    }())
                }
            ]
        });


}

