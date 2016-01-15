controllers.controller('homeController', ['$scope', '$location', function ($scope, $location) {
    $scope.navigateTo = function (repository) {
        $location.url('/discover/' + repository);
    }

    $scope.repositories = [
        {
            id: 'UWP',
            name: 'Universal Windows Platform',
            description: 'Windows 10 introduces the Universal Windows Platform (UWP), which further evolves the Windows Runtime model and brings it into the Windows 10 unified core. As part of the core, the UWP now provides a common app platform available on every device that runs Windows 10.'
        },
        {
            id: 'SOUWP',
            name: 'UWP On Stackoverflow',
            description: 'Windows 10 introduces the Universal Windows Platform (UWP), which further evolves the Windows Runtime model and brings it into the Windows 10 unified core. As part of the core, the UWP now provides a common app platform available on every device that runs Windows 10.'
        },
        {
            id: 'SOTFS',
            name: 'Team Foundation Server',
            description: 'Team Foundation Server is a complete Application Lifecycle Management (ALM) suite, offered by Microsoft.'
        },
        {
            id: 'SOVSO',
            name: 'Visual Studio Team Services',
            description: 'Visual Studio Team Services is a hosted Application Lifecycle Management solution from Microsoft which includes source control repositories (Git & TFVC), work item tracking, test case management, hosted automated builds, and more.'
        },
        {
            id: 'OFFICE',
            name: 'Microsoft Office',
            description: 'From home to business, from desktop to web and the devices in between, Office delivers the tools to get work done.'
        },
        {
            id: 'OFFICEDEV',
            name: 'Microsoft Office for Developers',
            description: 'Extend the functionality of Office (VSTO, Office Add-in and .etc)'
        },
        {
            id: 'SQL',
            name: 'Microsoft SQL Server',
            description: 'SQL Server is the foundation of Microsoft\'s data platform delivering mission critical performance with in-memory technologies and faster insights on any data ...'
        }
    ];
}]);