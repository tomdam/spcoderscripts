using Microsoft.SharePoint.Client.WorkflowServices;

public class WorkflowDetails
{
    public string Site {get; set;}
    public string List {get; set;}
    public string ContentType {get; set;}
    public string Workflow {get; set;}
    public string Type {get; set;}
}

public class SPSharePointWorkflowGetter : BasePlugin
{
    public SPSharePointWorkflowGetter()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.Web);
        this.Name = "Get Workflows";
    }

    public override void Execute(Object target)
    {
        List<WorkflowDetails> wfDetailsList = new List<WorkflowDetails> ();
        this.GetWorkflows((Microsoft.SharePoint.Client.Web)target, wfDetailsList);
        Result = wfDetailsList;
        ExecuteCallback(wfDetailsList);
    }
    

    public void GetWorkflows(Microsoft.SharePoint.Client.Web web, List<WorkflowDetails> wfDetailsList)
    {
        var ctx = web.Context;
        ctx.Load(web, w => w.Url); 
        ctx.Load(web.Webs);
        ctx.Load(web.Lists);
        ctx.Load(web.ContentTypes);
        ctx.ExecuteQuery();    
        Console.WriteLine("Checking " + web.Url);
        
        if (web.Lists != null && web.Lists.Count > 0)
        {
            foreach(var l in web.Lists)
            {
                ctx.Load(l.WorkflowAssociations);
                ctx.ExecuteQuery();
                
                if (l.WorkflowAssociations != null && l.WorkflowAssociations.Count > 0)
                {
                    foreach(var w in l.WorkflowAssociations)
                    {                 
                        if (w.Name.Contains("(Previous Version")) continue;
                        var wd      = new WorkflowDetails();
                        wd.Site     = web.Url;
                        wd.List     = l.Title;
                        wd.Workflow = w.Name;
                        wd.Type     = "2010";
                        wfDetailsList.Add(wd);
                    }
                }
            }        
        }
        
        if (web.ContentTypes != null && web.ContentTypes.Count > 0)
        {
            foreach(var ct in web.ContentTypes)
            {
                ctx.Load(ct.WorkflowAssociations);
                ctx.ExecuteQuery();
                
                if (ct.WorkflowAssociations != null && ct.WorkflowAssociations.Count > 0)
                {
                    foreach(var w in ct.WorkflowAssociations)
                    {
                        if (w.Name.Contains("(Previous Version")) continue;
                        var wd         = new WorkflowDetails();
                        wd.Site        = web.Url;
                        wd.ContentType = ct.Name;
                        wd.Workflow    = w.Name;
                        wd.Type        = "2010";
                        wfDetailsList.Add(wd);
                    }
                }
            }        
        }
        
        WorkflowServicesManager wfServicesManager         = new WorkflowServicesManager(ctx, web);
        WorkflowSubscriptionService wfSubscriptionService = wfServicesManager.GetWorkflowSubscriptionService();    
        WorkflowSubscriptionCollection wfSubscriptions    = wfSubscriptionService.EnumerateSubscriptions();
        ctx.Load(wfSubscriptions);
        ctx.ExecuteQuery();
        if (wfSubscriptions != null && wfSubscriptions.Count > 0)
        {
            foreach(var wf in wfSubscriptions)
            {
                var wd      = new WorkflowDetails();
                wd.Site     = web.Url;
                //wd.List     = l.Title;
                var myList = web.Lists.Where(m => m.Id == wf.EventSourceId).FirstOrDefault();
                if (myList != null)
                {
                    wd.List = myList.Title;
                }
                wd.Workflow = wf.Name;
                wd.Type     = "2013";
                wfDetailsList.Add(wd);
            }
        }

        
        if (web.Webs != null && web.Webs.Count > 0)
        {
            foreach (var w in web.Webs)
            {
                GetWorkflows(w, wfDetailsList);
            }
        }
    }
}

//registration code
SPSharePointWorkflowGetter sPSharePointWorkflowGetter = new SPSharePointWorkflowGetter();
sPSharePointWorkflowGetter.Callback += DoShowObjectInGrid;
PluginContainer.Register(sPSharePointWorkflowGetter);

logger.LogInfo("Registered plugin SPSharePointWorkflowGetter");
