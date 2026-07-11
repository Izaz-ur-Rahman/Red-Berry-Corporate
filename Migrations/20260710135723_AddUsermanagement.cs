using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedBerryCorporate.Migrations
{
    /// <inheritdoc />
    public partial class AddUsermanagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblEmployee",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PERSON_TYPE_ID = table.Column<int>(type: "int", nullable: true),
                    EMPLOYEE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OLD_EMPLOYEE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAYROLL_ID = table.Column<int>(type: "int", nullable: true),
                    PERSON_ID = table.Column<int>(type: "int", nullable: true),
                    BADGE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TITLE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIRST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SECOND_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    THIRD_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LAST_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIRST_NAME_AR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SECOND_NAME_AR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    THIRD_NAME_AR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LAST_NAME_AR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FULL_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FULL_NAME_AR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NATIONAL_IDENTIFIER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MANAGER_FLAG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SUPERVISOR_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ORGANIZATION_ID = table.Column<int>(type: "int", nullable: true),
                    JOB_ID = table.Column<int>(type: "int", nullable: true),
                    POSITION_ID = table.Column<int>(type: "int", nullable: true),
                    GRADE_ID = table.Column<int>(type: "int", nullable: true),
                    NATIONALITY_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SEX = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MARITAL_STATUS_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RELIGION_CODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SECT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DATE_OF_BIRTH = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ORIGINAL_DATE_OF_HIRE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HIRE_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ACTUAL_TERMINATION_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EMAIL_ADDRESS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MOBILE_SMS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MOBILE_M = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LOCATION_ID = table.Column<int>(type: "int", nullable: true),
                    OFFICE_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    INTERNAL_LOCATION = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CURRENT_FLAG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRIMARY_FLAG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ASSIGNMENT_TYPE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ASSIGNMENT_CATEGORY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EMPLOYMENT_CATEGORY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ASSIGNMENT_ID = table.Column<int>(type: "int", nullable: true),
                    ASSIGNMENT_NUMBER = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PAY_BASIS_ID = table.Column<int>(type: "int", nullable: true),
                    ASSIGNMENT_STATUS_TYPE_ID = table.Column<int>(type: "int", nullable: true),
                    PEOPLE_GROUP_ID = table.Column<int>(type: "int", nullable: true),
                    USER_ID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    USER_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PHONE_NOS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CONTRACT_END_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LAST_UPDATE_DATE_PEOPLE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LAST_UPDATE_DATE_ASSIGNMENTS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BUSINESS_GROUP_ID = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    HOD = table.Column<int>(type: "int", nullable: true),
                    EmployeeType = table.Column<int>(type: "int", nullable: true),
                    Shift = table.Column<int>(type: "int", nullable: true),
                    EducationId = table.Column<int>(type: "int", nullable: true),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyContactPhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Idenitification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MotherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfChild = table.Column<int>(type: "int", nullable: true),
                    ProfilePicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    WorkFromHome = table.Column<bool>(type: "bit", nullable: true),
                    ProfilePic = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ExcludefromTimesheet = table.Column<bool>(type: "bit", nullable: true),
                    WPS = table.Column<bool>(type: "bit", nullable: true),
                    GlobalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkSite = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SectID = table.Column<long>(type: "bigint", nullable: true),
                    BranchID = table.Column<int>(type: "int", nullable: true),
                    CountryID = table.Column<long>(type: "bigint", nullable: true),
                    Picture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    EmpDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChildCount = table.Column<int>(type: "int", nullable: true),
                    WFamily = table.Column<bool>(type: "bit", nullable: true),
                    DegreeHolder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpRankID = table.Column<long>(type: "bigint", nullable: true),
                    Religion = table.Column<int>(type: "int", nullable: true),
                    HouseRented = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountID = table.Column<int>(type: "int", nullable: true),
                    OrgChartID = table.Column<int>(type: "int", nullable: true),
                    IsSaleman = table.Column<bool>(type: "bit", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Target = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Session_time = table.Column<int>(type: "int", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createStatus = table.Column<bool>(type: "bit", nullable: true),
                    updateStatus = table.Column<bool>(type: "bit", nullable: true),
                    IsSyncWithBMS = table.Column<bool>(type: "bit", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkFromHomeDayWise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludedInSupervisorEmail = table.Column<bool>(type: "bit", nullable: true),
                    IncludedInAbsentEmail = table.Column<bool>(type: "bit", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Facebook = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Twitter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatsappNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReraNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Languages = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblEmployee", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmpId = table.Column<int>(type: "int", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DomainUser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: true),
                    InspectRole = table.Column<int>(type: "int", nullable: true),
                    AppId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AppInspection = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Language = table.Column<int>(type: "int", nullable: true),
                    AppWiseRoles = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AppIDs = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RoleNames = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DepartmentList = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblEmployee");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
