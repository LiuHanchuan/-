using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Tracking;
using Apache.Models;

namespace Apache.WorkFlow
{
    public class ItServiceBookMark : NativeActivity<string>
    {
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }
        public InArgument<string> CurrentUser { get; set; }
        public InArgument<string> OpinionField { get; set; }
        //public InArgument<string> CurrentRole { get; set; }

        public InArgument<WorkFlowInParameter> WorkFlowInParameters { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            var customRecord = new CustomTrackingRecord("CurrentUserRecord");
            customRecord.Data.Add("CurrentUser", CurrentUser.Get(context));
            //customRecord.Data.Add("CurrentRole", CurrentRole.Get(context));
            customRecord.Data.Add("Drafter", WorkFlowInParameters.Get(context).drafter);
            customRecord.Data.Add("OpinionField", OpinionField.Get(context) == null ? "" : OpinionField.Get(context));
            context.Track(customRecord);
            context.CreateBookmark(BookmarkName.Get(context), new BookmarkCallback(OnResumeBookmark));   //创建书签
        }

        protected override bool CanInduceIdle
        {
            get { return true; }
        }

        public void OnResumeBookmark(NativeActivityContext context, Bookmark bookmark, object obj)
        {
            // When the Bookmark is resumed, assign its value to
            // the Result argument.
            Result.Set(context, (string)obj);
        }
    }
}