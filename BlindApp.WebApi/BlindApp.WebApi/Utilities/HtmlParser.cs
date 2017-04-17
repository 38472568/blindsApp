using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BlindApp.WebApi.Utilities
{
    public class HtmlParser
    {
          private string[] htmlcode; //把html转为数组形式用于分析
          private StringBuilder result = new StringBuilder ();  //输出的结果
          private int seek; //分析文本时候的指针位置
          private string[] keepTag;  //用于保存要保留的尖括号内容
          private bool _inTag;  //标记现在的指针是不是在尖括号内
          private bool needContent = true;  //是否要提取正文
          private string tagName;  //当前尖括号的名字
          private string[] specialTag = new string[] { "script", "style", "!--" };  //特殊的尖括号内容，一般这些标签的正文是不要的
          
          /// <summary>
          /// 当指针进入尖括号内，就会触发这个属性。这里主要逻辑是提取尖括号里的标签名字
          /// </summary>
          public bool inTag {
              get { return _inTag; }
              set {
                  _inTag = value;
                  if (!value)
                      return;
                  bool ok = true;
                  tagName = "";
                  while (ok) {
                      string word = read ();
                      if (word != " " && word != ">") {
                          tagName += word;
                     } else if (word == " " && tagName.Length > 0) {
                          ok = false;
                      } else if (word == ">") {
                          ok = false;
                          inTag = false;
                          seek -= 1;
                      }
                  }
              }
          }
          /// <summary>
          /// 初始化类
          /// </summary>
          /// <param name="html">
          ///  要分析的html代码
          /// </param>
          public HtmlParser (string html)
          {
              htmlcode = new string[html.Length];
              for (int i = 0; i < html.Length; i++) {
                  htmlcode[i] = html[i].ToString ();
              }
              KeepTag (new string[] {  });
          }
          /// <summary>
          /// 设置要保存那些标签不要被过滤掉
          /// </summary>
          /// <param name="tags">
          ///
          /// </param>
          public void KeepTag (string[] tags)
          {
              keepTag = tags;
          }
          
          /// <summary>
          /// 
          /// </summary>
          /// <returns>
          /// 输出处理后的文本
          /// </returns>
          public string Text ()
          {
              int startTag = 0;
              int endTag = 0;
              while (seek < htmlcode.Length) {
                  string word = read ();
                  if (word.ToLower () == "<") {
                      startTag = seek;
                      inTag = true;
                  } else if (word.ToLower () == ">") {
                      endTag = seek;
                      inTag = false;
                      if (iskeepTag (tagName.Replace ("/", ""))) {
                         for (int i = startTag - 1; i < endTag; i++) {
                             result.Append (htmlcode[i].ToString ());
                         }
                     } else if (tagName.StartsWith ("!--")) {
                         bool ok = true;
                         while (ok) {
                             if (read () == "-") {
                                 if (read () == "-") {
                                     if (read () == ">") {
                                         ok = false;
                                     } else {
                                         seek -= 1;
                                     }
                                 }
                             }
                         }
                     } else {
                         foreach (string str in specialTag) {
                             if (tagName == str) {
                                 needContent = false;
                                 break;
                             } else
                                 needContent = true;
                         }
                     }
                 } else if (!inTag && needContent) {
                     result.Append (word);
                 }
                 
             }
             return result.ToString ();
         }
         /// <summary>
         /// 判断是否要保存这个标签
         /// </summary>
         /// <param name="tag">
         /// A <see cref="System.String"/>
         /// </param>
         /// <returns>
         /// A <see cref="System.Boolean"/>
         /// </returns>
         private bool iskeepTag (string tag)
         {
             foreach (string ta in keepTag) {
                 if (tag.ToLower () == ta.ToLower ()) {
                     return true;
                 }
             }
             return false;
         }
         private string read ()
         {
             return htmlcode[seek++];
         }
    }
}