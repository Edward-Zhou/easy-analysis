## Import Stackoverflow Repository Guidance  
#### Actions:

- SyncWithStackoverflow  (*Sync up stackoverflow data with external system*)

- SyncWithStackoverflowTags  (*Sync up tags for stackoverflow cases with external system*)

- BuildStackoverflowQuestionProfile  (*Build the stackoverflow threads profile*)

- AddMetadataToThreadProfile  (*Sync up Category & Tag*)

####Sample Command:

`EasyAnalysis.Backend.exe type:action name:sync-with-stackoverflow "parameters:SOUWP|2015-11-01T00:00:00Z&2015-11-30T00:00:00Z"`

`EasyAnalysis.Backend.exe type:action name:sync-with-stackoverflow-tags "parameters:SOUWP|2015-11-01T00:00:00Z&2015-11-30T00:00:00Z"`

`EasyAnalysis.Backend.exe type:action name:build-so-question-profile "parameters:souwp|souwp.thread_profiles|2015-11-01T00:00:00Z&2016-01-01T00:00:00Z"`

`EasyAnalysis.Backend.exe type:action name:add-metadata-to-threadprofile "parameters:souwp|souwp.thread_profiles|2015-11-01T00:00:00Z&2016-01-01T00:00:00Z"`

####JSON Configuration:

D:\souwp_import_actions.json:
```
[
  {
    "type": 2,
    "name": "sync-with-stackoverflow",
    "parameters": [
      "SOUWP",
      "2015-11-01T00:00:00Z&2015-12-31T00:00:00Z"
    ]
  },
  {
    "type": 2,
    "name": "sync-with-stackoverflow-tags",
    "parameters": [
      "SOUWP",
      "2015-11-01T00:00:00Z&2015-12-31T00:00:00Z"
    ]
  },
  {
    "type": 2,
    "name": "build-so-question-profile",
    "parameters": [
      "souwp",
      "souwp.thread_profiles",
      "2015-11-01T00:00:00Z&2016-01-01T00:00:00Z"
    ]
  },
  {
    "type": 2,
    "name": "add-metadata-to-threadprofile",
    "parameters": [
	    "souwp",
      "souwp.thread_profiles",
      "2015-11-01T00:00:00Z&2016-01-01T00:00:00Z"
    ]
  }
] 
```
Type command:
`EasyAnalysis.Backend.exe D:\souwp_import_actions.json`