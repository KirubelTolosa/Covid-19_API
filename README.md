# COVID-19 REST API
This covid-19 rest api can be used to look-up global covid related data, ConfirmedCases, Deaths, and Recoveries. Its a pipeline application that pulls data from "Johns Hopkins" repo, stores it a database and avails several endpoint to query the database. </br>

Hosting the application requires changing the endpoint to the databases and scheduling a task to update it with recent data. The running version of the application is hosted at aws elasticbeanstalk at the specified urls below and is scheduled to pull for updated data every five minutes. </br>
<i>(US, State and County level data will be included in the next release).</i>



*** The data source I used is provided by <i>"Johns Hopkins University_Center for Systems Science and Engineering (CSSE).</i>

### URI to access running versions of the application:
(Documentation included)
    <div>
      http://covidapi-env.eba-uiymmd7t.us-east-1.elasticbeanstalk.com/
      <br>
      www.kirubeltolosa.com
    </div>
  <h3>Here are a few example URI refernces to resource usage</h3>  
  <br>
    <pre>
    
    Use the following uri reference to get national count of cases(metrics) from all nations in the world.  
              "api/covid/Confirmed_Cases",
              "api/covid/Deaths",
              "api/covid/Recoveries"                  
              
    Use the following uri reference to get the worldwide count of cases(metrics). 
             "api/covid/{metrics}/GlobalCount"
                    
    Use the following uri reference to get the national count of cases(metrics) of a nation. Optionally, you can include a date to find the count of cases on that date. 
            "api/covid/{metrics}/Country/{Date : Optional}"
           
    Use the following uri refernce to get the totalled count of cases of nation for each tracked date. 
            "api/covid/{metrics}/Country/DailyCount"  
   </pre>
</br> 

 
<i>I hope you will find this useful and please reach out to me at info@kirubeltolosa.com if you have any question or feedback. Thanks. </i>
