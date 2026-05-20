# Flowcharts — BugTrackingSystem

---

## 1. Request Lifecycle (DI Flow)

```mermaid
flowchart TD
    A([HTTP Request]) --> B[IIS Express]
    B --> C[WebApiConfig\nMapHttpAttributeRoutes]
    C --> D[JwtAuthorizeAttribute\nValidate Token]
    D -->|Token Invalid| E([401 Unauthorized])
    D -->|Token Valid| F[AuthorizeRoleAttribute\nCheck Role]
    F -->|Role Not Allowed| G([403 Forbidden])
    F -->|Role Allowed| H[Controller\nConstructor Injection via Unity]
    H --> I[Service Method\nBusiness Logic]
    I --> J[BugTrackingDbContext\nEF6 Query]
    J --> K[(SQL Server\nDatabase)]
    K --> J
    J --> I
    I --> L{ServiceResult\nSuccess?}
    L -->|false| M[Controller\nReturn 400 / 404 / 403]
    L -->|true| N[Controller\nReturn 200 OK + Data]
    M --> O([HTTP Response])
    N --> O
```

---

## 2. Login Flow

```mermaid
flowchart TD
    A([POST /api/auth/login]) --> B[AuthController.Login]
    B --> C{ModelState\nValid?}
    C -->|No| D([400 Bad Request])
    C -->|Yes| E[AuthService.Login]
    E --> F[Query Users WHERE\nEmail + Password + NOT Deleted]
    F --> G{User\nFound?}
    G -->|No| H([200 - Success:false\nInvalid email or password])
    G -->|Yes| I[Update LastLoginAt\nSaveChanges]
    I --> J{MustChange\nPassword?}
    J -->|Yes| K([200 - MustChangePassword:true\nNo Token])
    J -->|No| L[JwtService.GenerateToken]
    L --> M([200 - Token + UserInfo])
```

---

## 3. Create Bug Flow

```mermaid
flowchart TD
    A([POST /api/bugs]) --> B[JwtAuthorize]
    B --> C[AuthorizeRole\nSystemAdmin / TestingManager / Tester]
    C --> D[BugsController.CreateBug]
    D --> E{ModelState\nValid?}
    E -->|No| F([400 Bad Request])
    E -->|Yes| G[BugsService.CreateBug]
    G --> H{Project Exists\nAND Not Deleted?}
    H -->|No| I([400 Project does not exist])
    H -->|Yes| J{Assigned User is\nActive Developer?}
    J -->|No| K([400 Must be active Developer])
    J -->|Yes| L[Insert Bug]
    L --> M[Insert Notification\nfor Developer]
    M --> N[Insert BugStatusHistory\nNone → New]
    N --> O[SaveChanges]
    O --> P([200 - BugId + Success Message])
```

---

## 4. Update Bug Status Flow

```mermaid
flowchart TD
    A([PUT /api/bugs/id/status]) --> B[JwtAuthorize]
    B --> C[AuthorizeRole\nSystemAdmin / DevelopmentManager / Developer]
    C --> D[BugsController.UpdateBugStatus]
    D --> E[BugsService.UpdateBugStatus]
    E --> F{Bug\nFound?}
    F -->|No| G([404 Not Found])
    F -->|Yes| H{Role == Developer\nAND Bug.AssignedTo != CurrentUser?}
    H -->|Yes| I([403 Forbidden\nOnly your bugs])
    H -->|No| J[Update Bug Status\nUpdatedBy + UpdatedAt]
    J --> K{NewStatus\n== Resolved?}
    K -->|Yes| L[Set ResolvedAt = Now]
    K -->|No| M[Skip]
    L --> N[Insert BugStatusHistory]
    M --> N
    N --> O[Insert Notification\nto Bug Creator]
    O --> P[SaveChanges]
    P --> Q([200 - OldStatus + NewStatus])
```

---

## 5. Retest Bug Flow

```mermaid
flowchart TD
    A([PUT /api/bugs/id/retest]) --> B[JwtAuthorize]
    B --> C[AuthorizeRole\nSystemAdmin / TestingManager / Tester]
    C --> D[BugsService.RetestBug]
    D --> E{Bug\nFound?}
    E -->|No| F([404 Not Found])
    E -->|Yes| G{Bug.Status\n== Resolved?}
    G -->|No| H([400 Only resolved bugs can be retested])
    G -->|Yes| I{IsFixed?}
    I -->|Yes| J[Status → Closed]
    I -->|No| K[Status → Reopened]
    J --> L[Insert BugStatusHistory]
    K --> L
    L --> M{TesterComment\nProvided?}
    M -->|Yes| N[Insert BugComment]
    M -->|No| O[Skip]
    N --> P{IsFixed?}
    O --> P
    P -->|No| Q[Insert Notification\nto Developer]
    P -->|Yes| R[Skip Notification]
    Q --> S[SaveChanges]
    R --> S
    S --> T([200 - Bug Closed / Reopened])
```

---

## 6. Upload Attachment Flow

```mermaid
flowchart TD
    A([POST /api/bugs/bugId/attachments]) --> B[JwtAuthorize]
    B --> C{Content-Type\nmultipart/form-data?}
    C -->|No| D([400 Bad Request])
    C -->|Yes| E[Map ~/Uploads/Attachments\nCreate if not exists]
    E --> F[ReadAsMultipartAsync\nSave temp files]
    F --> G{Files\nUploaded?}
    G -->|No| H([400 No file was uploaded])
    G -->|Yes| I[Build UploadFileInfo list]
    I --> J[AttachmentsService.Upload]
    J --> K{Bug Exists\nAND Not Deleted?}
    K -->|No| L([404 Not Found])
    K -->|Yes| M[For each file:\nGenerate Guid filename\nMove temp → final path\nInsert Attachment record]
    M --> N[SaveChanges]
    N --> O([200 - File uploaded successfully])
```

---

## 7. Soft Delete Flow (Project / Bug / Attachment / User)

```mermaid
flowchart TD
    A([DELETE /api/resource/id]) --> B[JwtAuthorize + AuthorizeRole]
    B --> C[Service.Delete]
    C --> D{Record Exists\nAND IsDeleted == false?}
    D -->|No| E([404 Not Found])
    D -->|Yes| F[Set IsDeleted = true\nDeletedBy = currentUserId\nDeletedAt = DateTime.Now]
    F --> G[SaveChanges]
    G --> H([200 - Deleted successfully])
    H --> I[Record still in DB\nbut hidden from all queries]
```

---

## 8. Unity DI — Dependency Injection Flow

```mermaid
flowchart LR
    A[Application_Start] --> B[UnityConfig.RegisterComponents]
    B --> C[UnityContainer]
    C --> D[Register BugTrackingDbContext\nHierarchicalLifetimeManager\nper-request scope]
    C --> E[Register IAuthService → AuthService]
    C --> F[Register IUsersService → UsersService]
    C --> G[Register IProjectsService → ProjectsService]
    C --> H[Register IBugsService → BugsService]
    C --> I[Register IDashboardService → DashboardService]
    C --> J[Register INotificationsService → NotificationsService]
    C --> K[Register IAttachmentsService → AttachmentsService]
    C --> L[Register IRolesService → RolesService]
    C --> M[GlobalConfiguration.DependencyResolver\n= UnityDependencyResolver]
    M --> N{HTTP Request arrives}
    N --> O[Unity creates child scope\nper request]
    O --> P[Inject BugTrackingDbContext\ninto Service Constructor]
    O --> Q[Inject IXxxService\ninto Controller Constructor]
    Q --> R[Controller handles request]
    R --> S[Request ends\nUnity disposes DbContext]
```
